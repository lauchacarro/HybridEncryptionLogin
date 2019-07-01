using System;
using System.IO;
using System.Security.Cryptography;
using HybridEncryptionLogin.Services.Abstracts;

namespace HybridEncryptionLogin.Services.Concretes
{
    public class RSAService : IRSAService
    {
        const string PEMPRIVATEHEADER = "-----BEGIN RSA PRIVATE KEY-----";
        const string PEMPRIVATEFOOTER = "-----END RSA PRIVATE KEY-----";
        const string PEMPUBLICHEADER = "-----BEGIN PUBLIC KEY-----";
        const string PEMPUBLICFOOTER = "-----END PUBLIC KEY-----";

        public byte[] DecryptString(string cipherText, string pemPrivateKey)
        {
            byte[] cipherTextData = Convert.FromBase64String(cipherText);

            using (RSACryptoServiceProvider provider = GetRSAProviderFromPEMPrivate(pemPrivateKey))
            {
                return provider.Decrypt(cipherTextData, false);
            }  
        }

        public RSACryptoServiceProvider GetRSAProviderFromPEMPrivate(string pem)
        {
            string pemstr = pem.Trim();
            if (!pemstr.StartsWith(PEMPRIVATEHEADER) || !pemstr.EndsWith(PEMPRIVATEFOOTER))
                return null;

            pemstr = pemstr.Replace(PEMPRIVATEHEADER, string.Empty).Replace(PEMPRIVATEFOOTER, string.Empty);
            RSAParameters rsaParameters = GetRSAProviderFromPEM(pemstr);
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportParameters(rsaParameters);
                return RSA;
            }
        }

        public string GetPrivatePEM(RSACryptoServiceProvider csp)
        {
            TextWriter outputStream = new StringWriter();

            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
            }

            return outputStream.ToString();
        }

        public string GetPublicPEM(RSACryptoServiceProvider csp)
        {
            TextWriter outputStream = new StringWriter();

            var parameters = csp.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);

                    for (int i = 0; i < 7; i++)
                    {
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    }

                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END PUBLIC KEY-----");

                return outputStream.ToString();
            }
        }

        public RSACryptoServiceProvider GetRSAProviderFromPEMPublic(string pemstr)
        {
            pemstr = pemstr.Trim();
            if (!pemstr.StartsWith(PEMPUBLICHEADER) || !pemstr.EndsWith(PEMPUBLICFOOTER))
                return null;

            pemstr = pemstr.Replace(PEMPUBLICHEADER, string.Empty).Replace(PEMPUBLICFOOTER, string.Empty).Trim();


            RSAParameters rsaParameters = GetRSAProviderFromPEM(pemstr);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(new RSAParameters()
            {
                Modulus = rsaParameters.Modulus,
                Exponent = rsaParameters.Exponent
            });
            return RSA;
        }

        RSAParameters GetRSAProviderFromPEM(string pemstr)
        {
            int pemLength = pemstr.Length;

            byte[] buffer = new byte[((pemLength * 3) + 3) / 4 -
        (pemLength > 0 && pemstr[pemLength - 1] == '=' ?
        pemLength > 1 && pemstr[pemLength - 2] == '=' ?
            2 : 1 : 0)];

            bool success = Convert.TryFromBase64String(pemstr, buffer, out _);

            if (success)
            {
                byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

                // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
                using (MemoryStream mem = new MemoryStream(buffer))
                {
                    using (BinaryReader binr = new BinaryReader(mem))
                    {
                        //wrap Memory Stream with BinaryReader for easy reading
                        try
                        {
                            ushort twobytes = binr.ReadUInt16();
                            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                                binr.ReadByte();    //advance 1 byte
                            else if (twobytes == 0x8230)
                                binr.ReadInt16();   //advance 2 bytes
                            else
                                return new RSAParameters();

                            twobytes = binr.ReadUInt16();
                            if (twobytes != 0x0102) //version number
                                return new RSAParameters();
                            byte bt = binr.ReadByte();
                            if (bt != 0x00)
                                return new RSAParameters();

                            int elems = GetIntegerSize(binr);
                            MODULUS = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            E = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            D = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            P = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            Q = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            DP = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            DQ = binr.ReadBytes(elems);

                            elems = GetIntegerSize(binr);
                            IQ = binr.ReadBytes(elems);


                            RSAParameters RSAparams = new RSAParameters();
                            RSAparams.Modulus = MODULUS;
                            RSAparams.Exponent = E;
                            RSAparams.D = D;
                            RSAparams.P = P;
                            RSAparams.Q = Q;
                            RSAparams.DP = DP;
                            RSAparams.DQ = DQ;
                            RSAparams.InverseQ = IQ;

                            return RSAparams;
                        }
                        catch (Exception)
                        {
                            return new RSAParameters();
                        }
                        finally { binr.Close(); }
                    }
                }
                
            }
            else
            {
                return new RSAParameters();
            }
        }

        private bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            int count;
            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                byte highbyte = binr.ReadByte();
                byte lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }



            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);     //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        private void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }


    }
}
