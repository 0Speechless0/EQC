using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace EQC.Common
{
    public  class Encryption
    {
		private  int mod;
		private int encrypKey;
		private int decrypKey;
		private Stack<bool> calStack;
		public Encryption()
		{
			int[] key = ConfigurationManager.AppSettings["SymmetricKey"].ToString().Split(',').Select(r => Int32.Parse(r)).ToArray();
			decrypKey = key[0] - key[1];
			encrypKey = key[1];
			mod = key[0];
			calStack = new Stack<bool>();
		}

		public string encryptCode(string seed)
        {
			var A = Encoding.UTF8.GetBytes(seed);


			var hashA = new MD5CryptoServiceProvider().ComputeHash(A);

			var B =
				hashA.ToList().Select(r => {
					convertToCalStack(encrypKey, calStack);

					return calRemainder(r, mod, calStack);

				}).ToArray();

			var _B = B.Aggregate("", (a, c) => a + c + ":");
			 _B = _B.Remove(_B.Length - 1, 1);

			var _A= A.Aggregate("", (a, c) => a + c + ":");
            _A= _A.Remove(_A.Length - 1, 1);


            return _B + "," + _A ;
		}

		public string decryptCode(string authCode)
        {
			string[] authdData = authCode.Split(',').ToArray();

			double[] encryptionCode = authdData[0]
			.Split(':')
			.Select(r => Double.Parse(r) ).ToArray();

			Byte[] originCode = authdData[1]
				.Split(':')
				.Select(r => Byte.Parse(r)).ToArray()
				.ToArray();

			var originHash = new MD5CryptoServiceProvider()
				.ComputeHash(originCode)
				.Select(r => (double)r)
				.ToArray();


			int i = 0;
			double[] decryptCode = originHash
				   .Select(r =>
				   {
					   convertToCalStack(decrypKey, calStack);
					   return (encryptionCode[i++]* calRemainder(r, mod, calStack)) % mod;
				   }).ToArray();

			if(!checkCodeVaild(decryptCode, originHash))
            {
				return null;
            }
			return Encoding.UTF8.GetString(originCode.Select(r => r).ToArray());
 
		}
		public static bool checkCodeVaild(double[] source, double[] target)
		{
			int i = 0;
			if(source.Count() == target.Count())
            {
				while(i < source.Count())
                {

					if (source[i] != target[i]) return false ;
					i++;
                }

			}
            else
            {
				return false;
            }

			return true;
		}
		private void convertToCalStack(int key, Stack<bool> calStack)
		{
			int n = key;


			while (n > 0)
			{
				if (n % 2 > 0)
				{
					calStack.Push(true);
				}
				else
				{
					calStack.Push(false);
				}

				n = n / 2;


			}

		}


		private  double calRemainder(double number, int mod, Stack<bool> calStack)
		{
			double calBase = number;
			List<double> calResult = new List<double>();

			for (int i = 0; i < calStack.Count; i++)
			{
				calResult.Add(calBase);
				calBase = Math.Pow(calBase, 2) % mod;

			}

			calBase = 1;
			do
			{
				int resultIndex = calStack.Count - 1;
				bool calSource = calStack.Pop();
				if (calSource)
				{
					Console.Write(calBase + "*" + calResult[resultIndex] + ",");
					calBase = (calBase * calResult[resultIndex]) % mod;

				}

			} while (calStack.Count > 0);

			return calBase;


		}
	}
}