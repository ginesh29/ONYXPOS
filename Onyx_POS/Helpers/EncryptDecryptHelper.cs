using System.Text;

namespace Onyx_POS.Helpers
{
    public static class EncryptDecryptHelper
    {
        public static string Encrypt(this string key)
        {
            string rnd = "$12TcA#";
            string encryptedstring = "";
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] preHash = Encoding.UTF8.GetBytes(key + rnd);
            byte[] hash = sha.ComputeHash(preHash);
            encryptedstring = Convert.ToBase64String(hash, 0, 15);
            encryptedstring = encryptedstring[..10];
            return encryptedstring;
        }
        //public static string Decrypt(string key)
        //{
        //    byte[] data;
        //    string decryptedstring = "";
        //    int CNTR = 1;
        //    data = Convert.FromBase64String(key);
        //    decryptedstring = Encoding.ASCII.GetString(data);
        //    // Dpass=Dpass+char ((Asc (Mid( Trim(EPass),CNTR,1))+17))
        //    //while (CNTR <= key.Length )
        //    //    {
        //    //     char cStr=   Mid(key.Trim (), CNTR, 1);
        //    //         decryptedstring = decryptedstring +(char) (Asc(cStr) + 17);
        //    //            CNTR = CNTR + 1;
        //    //    }
        //    return decryptedstring;
        //}

        //public static char Mid(string param, int startIndex, int length)
        //{
        //    string result = param.Substring(startIndex, length);
        //    char[] charArr = result.ToCharArray();
        //    return charArr[0];
        //}
        //public static int Asc(char String)
        //{
        //    int num;
        //    byte[] numArray;
        //    int num1 = Convert.ToInt32(String);
        //    if (num1 >= 128)
        //    {
        //        try
        //        {
        //            Encoding fileIOEncoding = Encoding.Default;
        //            char[] str = new char[] { String };
        //            if (!fileIOEncoding.IsSingleByte)
        //            {
        //                numArray = new byte[2];
        //                if (fileIOEncoding.GetBytes(str, 0, 1, numArray, 0) != 1)
        //                {
        //                    if (BitConverter.IsLittleEndian)
        //                    {
        //                        byte num2 = numArray[0];
        //                        numArray[0] = numArray[1];
        //                        numArray[1] = num2;
        //                    }
        //                    num = BitConverter.ToInt16(numArray, 0);
        //                }
        //                else
        //                {
        //                    num = numArray[0];
        //                }
        //            }
        //            else
        //            {
        //                numArray = new byte[1];
        //                fileIOEncoding.GetBytes(str, 0, 1, numArray, 0);
        //                num = numArray[0];
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            throw exception;
        //        }
        //    }
        //    else
        //    {
        //        num = num1;
        //    }
        //    return num;
        //}
    }
}
