﻿using NUnit.Framework;
using NUnit.Framework.Internal;

using System.Reflection;
using System.Text;

using static IReves.Network.Tests.TestHelper;

namespace IReves.Network.Tests
{
    [TestFixture]
    public class ReadWriteTests : PeerTestFixture
    {
        [Test]
        public void TestReadWrite()
        {
            NetOutgoingMessage msg = Peer.CreateMessage();

            msg.Write(false);
            msg.Write(-3, 6);
            msg.Write(42);
            msg.Write("duke of earl");
            msg.Write((byte) 43);
            msg.Write((ushort) 44);
            msg.Write(ulong.MaxValue, 64);
            msg.Write(true);

            msg.WritePadBits();

            int bcnt = 0;

            msg.Write(567845.0f);
            msg.WriteVariableInt32(2115998022);
            msg.Write(46.0);
            msg.Write((ushort) 14, 9);
            bcnt += msg.WriteVariableInt32(-47);
            msg.WriteVariableInt32(470000);
            msg.WriteVariableUInt32(48);
            bcnt += msg.WriteVariableInt64(-49);

            if (bcnt != 2)
                Assert.Fail("WriteVariable* wrote too many bytes!");

            byte[] data = msg.Data;

            NetIncomingMessage inc = CreateIncomingMessage(data, msg.LengthBits);

            StringBuilder bdr = new StringBuilder();

            bdr.Append(inc.ReadBoolean());
            bdr.Append(inc.ReadInt32(6));
            bdr.Append(inc.ReadInt32());

            string strResult;
            bool ok = inc.ReadString(out strResult);
            if (ok == false)
                Assert.Fail("Read/write failure");

            bdr.Append(strResult);

            bdr.Append(inc.ReadByte());

            if (inc.PeekUInt16() != 44)
                Assert.Fail("Read/write failure");

            bdr.Append(inc.ReadUInt16());

            if (inc.PeekUInt64(64) != ulong.MaxValue)
                Assert.Fail("Read/write failure");

            bdr.Append(inc.ReadUInt64());
            bdr.Append(inc.ReadBoolean());

            inc.SkipPadBits();

            bdr.Append(inc.ReadSingle());
            bdr.Append(inc.ReadVariableInt32());
            bdr.Append(inc.ReadDouble());
            bdr.Append(inc.ReadUInt32(9));
            bdr.Append(inc.ReadVariableInt32());
            bdr.Append(inc.ReadVariableInt32());
            bdr.Append(inc.ReadVariableUInt32());
            bdr.Append(inc.ReadVariableInt64());

            if (bdr.ToString()
                .Equals("False-342duke of earl434418446744073709551615True56784521159980224614-4747000048-49"))
            {
                // Console.WriteLine("Read/write tests OK");
            }
            else
            {
                Assert.Fail("Read/write tests FAILED!");
            }

            msg = Peer.CreateMessage();

            NetOutgoingMessage tmp = Peer.CreateMessage();
            tmp.Write(42, 14);

            msg.Write(tmp);
            msg.Write(tmp);

            if (msg.LengthBits != tmp.LengthBits * 2)
                Assert.Fail("NetOutgoingMessage.Write(NetOutgoingMessage) failed!");

            tmp = Peer.CreateMessage();

            var test = new Test {Number = 42, Name = "Hallon", Age = 8.2f};

            tmp.WriteAllFields(test, BindingFlags.Public | BindingFlags.Instance);

            data = tmp.Data;

            inc = CreateIncomingMessage(data, tmp.LengthBits);

            var readTest = new Test();
            inc.ReadAllFields(readTest, BindingFlags.Public | BindingFlags.Instance);

            NetException.Assert(readTest.Number == 42);
            NetException.Assert(readTest.Name == "Hallon");
            NetException.Assert(readTest.Age == 8.2f);

            // test aligned WriteBytes/ReadBytes
            msg = Peer.CreateMessage();
            byte[] tmparr = {5, 6, 7, 8, 9};
            msg.Write(tmparr);

            inc = CreateIncomingMessage(msg.Data, msg.LengthBits);
            byte[] result = inc.ReadBytes(tmparr.Length);

            for (int i = 0; i < tmparr.Length; i++)
                if (tmparr[i] != result[i])
                    Assert.Fail("readbytes fail");
        }
    }
    public class TestBase
    {
        public int Number;
    }

    public class Test : TestBase
    {
        public float Age;
        public string Name;
    }
}