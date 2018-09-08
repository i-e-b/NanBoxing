using System;
using System.Collections.Generic;
using NanBoxing;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class NanBoxingTests
    {
        [Test]
        public void doubles_are_interpreted_as_numeric () {
            var rnd = new Random();
            for (int i = 0; i < 10000; i++)
            {
                var d = rnd.NextDouble() / i;
                var inv = 1.0d / d + double.Epsilon;

                Assert.That(NanTags.TypeOf(d), Is.EqualTo(DataType.Number));
                Assert.That(NanTags.TypeOf(inv), Is.EqualTo(DataType.Number));
            }
        }

        [Test]
        public void opcodes_survive_a_round_trip ()
        {
            double enc1 = NanTags.EncodeOpcode('c','j', 123, 0); // control, jump, 123, <unused>
            double enc2 = NanTags.EncodeOpcode('f','d', 3, 40); // function, define, 3 params, 40 opcodes

            Assert.That(NanTags.TypeOf(enc1), Is.EqualTo(DataType.Opcode));
            Assert.That(NanTags.TypeOf(enc2), Is.EqualTo(DataType.Opcode));

            NanTags.DecodeOpCode(enc1, out var codeClass, out var codeAction, out var p1, out var p2);
            Assert.That(codeClass, Is.EqualTo('c'));
            Assert.That(codeAction, Is.EqualTo('j'));
            Assert.That(p1, Is.EqualTo(123));
            Assert.That(p2, Is.EqualTo(0));

            NanTags.DecodeOpCode(enc2, out codeClass, out codeAction, out p1, out p2);
            Assert.That(codeClass, Is.EqualTo('f'));
            Assert.That(codeAction, Is.EqualTo('d'));
            Assert.That(p1, Is.EqualTo(3));
            Assert.That(p2, Is.EqualTo(40));
        }

        [Test]
        public void identifier_names_can_be_encoded_and_survive_a_round_trip () {
            var enc = NanTags.EncodeVariableRef("HelloWorld", out var crush);
            var type = NanTags.TypeOf(enc);

            var enc2 = NanTags.EncodeVariableRef("Hel" + "lo" + "Wo" + 'r' + 'l' + 'd', out var crush2);
            var other = NanTags.EncodeVariableRef("HelloWorld2", out var crushOther);

            Console.WriteLine("Crush:   " + crush.ToString("X"));
            Console.WriteLine("Crush:   " + crushOther.ToString("X"));

            var checkCrush = NanTags.DecodeVariableRef(enc);

            Assert.That(checkCrush, Is.EqualTo(crush));

            Assert.That(type, Is.EqualTo(DataType.VariableRef));
            Assert.That(NanTags.AreEqual(enc, enc2), Is.True);
            Assert.That(NanTags.AreEqual(enc, other), Is.False);
            Assert.That(crush, Is.EqualTo(crush2));
            Assert.That(crush, Is.Not.EqualTo(crushOther));
        }

        [Test]
        public void short_identifier_names_get_encoded_uniquely (){
            var seen = new HashSet<ulong>();
            for (int i = 0; i < 60; i++)
            {
                var cs = ((char)('A'+i)).ToString();
                NanTags.EncodeVariableRef(cs, out var crush);
                Assert.That(seen.Contains(crush), Is.False);
                seen.Add(crush);
            }

            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var cs = ((char)('A' + i)).ToString() + j;
                    NanTags.EncodeVariableRef(cs, out var crush);
                    Assert.That(seen.Contains(crush), Is.False);
                    seen.Add(crush);
                }
            }
        }

        [Test]
        public void pointer_encoding_survives_a_round_trip () {
            var rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {

                long target = rnd.Next() + ((long)rnd.Next() << 15);
                var type = DataType.PtrArray_String;

                var enc = NanTags.EncodePointer(target, type);

                NanTags.DecodePointer(enc, out var newTarget, out var newType);


                Assert.That(newTarget, Is.EqualTo(target), "Multiplier: " + i);
                Assert.That(newType, Is.EqualTo(type));
            }
        }

        [Test]
        public void int32_overflows_the_same_way_as_native () {
            unchecked{
                for (int i = 1; i > 0; i += i)
                {
                    int original = i * 2;
                    double enc = NanTags.EncodeInt32(original);
                    int result = NanTags.DecodeInt32(enc);

                    Assert.That(result, Is.EqualTo(original));
                }
            }
        }

        [Test]
        public void encoding_and_decoding_short_strings (){ }

        [Test]
        public void encoding_and_decoding_unsigned_int32 (){
            unchecked{
                for (int i = 1; i > 0; i += i)
                {
                    uint original = (uint)i * 3;
                    double enc = NanTags.EncodeUInt32(original);
                    uint result = NanTags.DecodeUInt32(enc);

                    Assert.That(result, Is.EqualTo(original));
                }
            }
        }
    }
}
