using EQC.EDMXModel;
using EQC.ViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EQC.Services
{
    public class APIService
    {

        public static Dictionary<string, Token> tokens { get; set; } = new Dictionary<string, Token>();


        public APIService()
        {

        }
        public  class Token
        {
            public byte[] pwdHashing { get; set; }
            public byte[] IVHashing { get; set; }
            public byte[] cipherText { get; set; }
            public string userNo { get; set; }
            public string Value { get; set; }
            public DateTime CreateTime { get; set; }

            public string bindingCode { get; set; }
        }

        public Token GetTokenData(string token)
        {
            Token myToken =
                tokens.Select(t => t.Value)
                .Where(t => t.Value == token).FirstOrDefault();
            return myToken;
        }
            public bool checkTokenVaild(string token)
            {

            if (token == null)
            {
                return false;
            }
            //token是否存在
            Token myToken = 
                tokens.Select(t => t.Value)
                .Where(t => t.Value  == token).FirstOrDefault();
            if (myToken == null)
            {
                return false;
            }
            string assertUserNo = DecryptStringFromBytes_Aes(
                myToken.cipherText, myToken.pwdHashing, myToken.IVHashing);


            //檢查是否在有效時間內
            if (myToken.CreateTime < DateTime.UtcNow.AddHours(-1))
            {

                return false;
            }

            //驗證失敗
            if (assertUserNo != myToken.userNo) return false;

            return true;

        }

        internal void addToken(Token token)
        {
            if (tokens.ContainsKey(token.userNo))
            {

                tokens.Remove(token.userNo);
            }
            else
            {
                tokens.Add(token.userNo, token);
            }

        }

        public Token addTokenWithBindingCode<T>(string userNo, string pwd, string clientBindingCode,
            Func<EQC_NEW_Entities, DbSet<T> > bindingCodeTableSelector
            
            )
        where T : class, BindingCodeVM, new()
        {
            using (var context = new EQC_NEW_Entities())
            {
                var userSeq = context.UserMain.Where(r => r.UserNo == userNo).First().Seq;
                var Lock = 
                    bindingCodeTableSelector
                    .Invoke(context)
                    .Where(r => r.UserMainSeq == userSeq ).FirstOrDefault();

                var bindingCode = bindingCodeTableSelector
                    .Invoke(context)
                    .ToList()
                    .Where(r => r.AppCode == clientBindingCode)
                    .FirstOrDefault();
                if(Lock == null)
                {
                    //註冊
                    bindingCodeTableSelector
                    .Invoke(context)
                    .Add(new T
                    {
                        AppCode = clientBindingCode,
                        UserMainSeq = userSeq

                    });
                    context.SaveChanges();
                }
                else if(Lock.AppCode != clientBindingCode  )
                {
                    throw new Exception("routine:該帳號已註冊AppCode，請洽詢客服協助處理，或登入品管系統的使用者管理中解鎖");
                }

                var newToken = addToken(userNo, pwd);
                newToken.bindingCode = clientBindingCode;

                return newToken;

            }
        }
        internal Token addToken(string userNo, string pwd, bool codeBinding = false)
        {
            byte[] pwdKey = pwd.ToCharArray()
                .Select(c => (byte)c)
                .ToArray();
            byte[] IV = DateTime.Now.ToLongTimeString()
                .Select(c => (byte)c)
                .ToArray();
            if (pwdKey == null) throw new Exception("該帳號未註冊");
            var pwdHashing = new MD5CryptoServiceProvider().ComputeHash(pwdKey);
            var IVHashing = new MD5CryptoServiceProvider().ComputeHash(IV);
            StringBuilder tokenBuilder = new StringBuilder();

            byte[] cipherText = EncryptStringToBytes_Aes(userNo, pwdHashing, IVHashing);

            cipherText.ToList()
                .ForEach(b => tokenBuilder.Append(b.ToString("x2")));
            string token = tokenBuilder.ToString();
            if (tokens.ContainsKey(userNo))
            {

                tokens.Remove(userNo);
            }
            Token newToken = new Token
            {
                cipherText = cipherText,
                pwdHashing = pwdHashing,
                IVHashing = IVHashing,
                userNo = userNo,
                Value = token,
                CreateTime = DateTime.Now
            };
            tokens.Add(userNo, newToken);
            return newToken;
        }

        internal int removeToken(string token)
        {
            string userNo =  tokens.ToList().Find(item => item.Value.Value == token).Key;
            if(userNo == null)
            {
                return 0;
            }
            if (tokens.Remove(userNo))
            {
                return  1;
            }

            return -1;

        }
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}