﻿using Ipfs.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Api
{

    [TestClass]
    public class PubSubApiTest
    {

        [TestMethod]
        public void Api_Exists()
        {
            IpfsClient ipfs = TestFixture.Ipfs;
            Assert.IsNotNull(ipfs.PubSub);
        }

        [TestMethod]
        public void Peers()
        {
            var ipfs = TestFixture.Ipfs;
            var peers = ipfs.PubSub.PeersAsync().Result.ToArray();
            Assert.IsTrue(peers.Length > 0);
        }

        [TestMethod]
        public void Subscribed_Topics()
        {
            var ipfs = TestFixture.Ipfs;
            var topics = ipfs.PubSub.SubscribedTopicsAsync().Result.ToArray();
            // TODO: Assert.IsTrue(peers.Length > 0);
        }

        volatile int messageCount = 0;

        [TestMethod]
        public async Task Subscribe()
        {
            messageCount = 0;
            var ipfs = TestFixture.Ipfs;
            var topic = "net-ipfs-api-test-" + Guid.NewGuid().ToString();
            await ipfs.PubSub.Subscribe(topic, msg =>
            {
                Interlocked.Increment(ref messageCount);
            });
            await ipfs.PubSub.Publish(topic, "hello world!");

            await Task.Delay(1000);
            Assert.AreEqual(1, messageCount);
         }

        volatile int messageCount1 = 0;

        [TestMethod]
        public async Task Unsubscribe()
        {
            messageCount1 = 0;
            var ipfs = TestFixture.Ipfs;
            var topic = "net-ipfs-api-test-" + Guid.NewGuid().ToString();
            var cs = new CancellationTokenSource();
            await ipfs.PubSub.Subscribe(topic, msg =>
            {
                Interlocked.Increment(ref messageCount1);
            }, cs.Token);
            await ipfs.PubSub.Publish(topic, "hello world!");
            await Task.Delay(1000);
            Assert.AreEqual(1, messageCount1);

            cs.Cancel();
            await ipfs.PubSub.Publish(topic, "hello world!!!");
            await Task.Delay(1000);
            Assert.AreEqual(1, messageCount1);
        }
    }
}
