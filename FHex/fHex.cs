using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Collections;

namespace FHex
{
    public class fBit
    {
        private BitArray bitArray;

        public fBit()
        {
            this.bitArray = new BitArray(0);
        }

        public fBit(bool bit)
        {
            this.bitArray = new BitArray(new bool[] { bit });
        }

        public fBit(char Char) 
        {
            string String = Char.ToString();
            this.bitArray = new fBit(String).bitArray;
        }
        public fBit(string String)
        {
            fHex fHex = new fHex(String);

            string binstr = fHex.ToBinaryStringNew();

            this.bitArray = FromBinaryString(binstr).bitArray;
        }
        public fBit(BitArray bits)
        {
            this.bitArray = bits;
        }
        public int Length => this.bitArray.Length;
        public void Reverse() 
        {
            var bools = ToBoolArray().ToList();
            bools.Reverse();
            FromBoolArray(bools.ToArray());
        }
        public BitArray GetBitArray() 
        {
            return this.bitArray;
        }
        public fBit Reversed()
        {
            fBit fBit = new fBit();
            var bools = ToBoolArray().ToList();
            bools.Reverse();
            fBit.FromBoolArray(bools.ToArray());
            return fBit;
        }
        public void AddBit(bool bit)
        {
            var currentLength = bitArray.Length;
            var newLength = currentLength + 1;
            var newArray = new BitArray(newLength);

            for (int i = 0; i < currentLength; i++)
            {
                newArray[i] = bitArray[i];
            }

            newArray[currentLength] = bit;
            bitArray = newArray;
        }
        public void SetBit(int index,bool bit)
        {
            var currentLength = bitArray.Length;
            var newArray = new BitArray(currentLength);

            for (int i = 0; i < currentLength; i++)
            {
                newArray[i] = bitArray[i];
            }

            newArray[index] = bit;

            bitArray = newArray;
        }
        public void AddBits(bool[] bits)
        {
            var currentLength = bitArray.Length;
            var newLength = currentLength + bits.Length;
            var newArray = new BitArray(newLength);

            for (int i = 0; i < currentLength; i++)
            {
                newArray[i] = bitArray[i];
            }

            for (int i = currentLength; i < newLength; i++)
            {
                newArray[i] = bits[i- currentLength];
            }

            bitArray = newArray;
        }
        public void AddBits(int amount,bool value = false)
        {
            var currentLength = bitArray.Length;
            var newLength = currentLength + amount;
            var newArray = new BitArray(newLength);

            for (int i = 0; i < currentLength; i++)
            {
                newArray[i] = bitArray[i];
            }

            for (int i = currentLength; i < newLength; i++)
            {
                newArray[i] = value;
            }

            bitArray = newArray;
        }
        public void AddBits(fBit bits)
        {
            AddBits(bits.ToBoolArray());
        }
        public fBit GetBetween(int start, int end)
        {
            string binaryString = "";
            List<string> bits = Split(ToString(),1).ToList();
            for (int i = start; i < end; i++)
            {
                binaryString += bits[i];
            }

            return fBit.FromBinaryString(binaryString);
        }
        public static List<bool> GetTrimmedBits(int number, int trim)
        {
            List<bool> bitList = new List<bool>();

            while (number > 0)
            {
                bitList.Insert(0, (number & 1) == 1);
                number >>= 1;
            }

            while (bitList.Count < trim)
            {
                bitList.Insert(0, false);
            }

            if (bitList.Count > trim)
            {
                bitList.RemoveRange(trim, bitList.Count - trim);
            }

            return bitList;
        }
        public static int FromTrimmedBits(IEnumerable<bool> bitThing,int trim)
        {
            var bitList = bitThing.ToList();
            // Pad leading zeros if necessary
            while (bitList.Count < trim)
            {
                bitList.Insert(0, false);
            }
            int number = 0;

            for (int i = 0; i < bitList.Count; i++)
            {
                number <<= 1;
                if (bitList[i])
                {
                    number |= 1;
                }
            }

            return number;
        }
        public static fBit FromBinaryString(string BinaryString) 
        {
            fBit fBit = new fBit();

            foreach (string bit in Split(BinaryString, 1))
            {
                if (bit == "0")
                    fBit.AddBit(false);
                else if (bit == "1")
                    fBit.AddBit(true);
                else
                    throw new Exception($@"incorrect bit! (expected either a 1 or a 0, got ""{bit}"")");
            }
            return fBit;
        }
        public byte[] ToByteArray()
        {
            string input = ToString();
            try_again:
            if ((input.Length % 8) != 0)
            {
                input += "0";
                goto try_again;
            }

            int numBytes = input.Length/8;
            byte[] bytes = new byte[numBytes];
            for (int i = 0; i < numBytes; ++i)
            {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }
            return bytes;
        }
        public byte[] ToReversedByteArray()
        {
            int numBytes = (bitArray.Length + 7) / 8;
            byte[] bytes = new byte[numBytes];
            bitArray.CopyTo(bytes, 0);
            Array.Reverse(bytes); // Reverse the byte array if necessary
            return bytes;
        }


        public bool[] ToBoolArray()
        {
            bool[] bools = new bool[bitArray.Length];
            bitArray.CopyTo(bools, 0);
            return bools;
        }
        public void FromBoolArray(bool[] bools)
        {
            bitArray = new BitArray(bools);
        }

        public override string ToString()
        {
            string output = "";
            foreach (bool bit in this.ToBoolArray())
            {
                output += bit ? "1" : "0";
            }
            return output;
        }
        public static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }

    public class fHex
    {
        private byte[] data;
        public fHex()
        {
            this.data = new byte[0];
        }
        public fHex(int num)
        {
            this.data = BitConverter.GetBytes(num);
        }
        public fHex(string text)
        {
            this.data = Encoding.UTF8.GetBytes(text);
        }
        public fHex(byte[] bytes)
        {
            this.data = bytes;
        }
        public fHex(Dictionary<string, string> OptionsDictionary)
        {
            string compiledDict = fHex.FromDictionary(OptionsDictionary);

            this.data = new fHex(compiledDict).GetBytes();
        }
        public int Length => this.data.Length;
        public bool IsEmpty()
        {
            return (this.data.Length == 0);
        }
        public void AddNumber(int num)
        {
            AddByteArray(BitConverter.GetBytes(num));
        }
        public void SetFromNumber(int num)
        {
            SetFromByteArray(BitConverter.GetBytes(num));
        }
        public void IncrementNumber(int num)
        {
            int newNumber = 0;
            newNumber += ToNumber();
            newNumber += num;

            SetFromByteArray(BitConverter.GetBytes(newNumber));
        }
        //public void AddBits(fBit bits)
        //{
        //    byte[] bytes = bits.ToByteArray();
        //    AddByteArray(bytes);
        //}
        public void AddString(string text)
        {
            AddByteArray(Encoding.UTF8.GetBytes(text));
        }
        public void SetFromString(string text)
        {
            SetFromByteArray(Encoding.UTF8.GetBytes(text));
        }
        public void PreAddByteArray(byte[] extraBytes)
        {
            List<byte> tempList = new List<byte>();
            tempList = extraBytes.ToList();
            tempList.AddRange(data.ToList());
            this.data = tempList.ToArray();
        }
        public void AddByteArray(byte[] extraBytes)
        {
            List<byte> tempList = new List<byte>();
            tempList = data.ToList();
            tempList.AddRange(extraBytes);
            this.data = tempList.ToArray();
        }
        //public string ToBinaryStringOld()
        //{
        //    string final = "";
        //    foreach (byte b in this.data)
        //    {
        //        string hexString = b.ToString("X2");
        //        string binaryString = Convert.ToString(Convert.ToInt32(hexString, 16), 2);
        //        final += binaryString.PadLeft(8, '0');
        //    }
        //    return final;
        //}
        //public string ToBinaryString()
        //{
        //    string final = "";
        //    foreach (byte b in this.data)
        //    {
        //        string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
        //        //final = binaryString + final;
        //        final += binaryString;
        //    }


        //    string final2 = "";
        //    foreach (string bit in fBit.Split(final,1))
        //    {
        //        final2 = bit + final2;
        //    }
        //    return final2;
        //}
        public string ToBinaryStringNew()
        {
            StringBuilder final = new StringBuilder();
            foreach (byte b in this.data)
            {
                string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
                final.Append(binaryString);
            }

            return final.ToString();
        }

        public void SetFromDictionary(Dictionary<string, string> OptionsDict)
        {
            this.data = new fHex(OptionsDict).GetBytes();
        }
        public void PreAddfHex(fHex extra)
        {
            PreAddByteArray(extra.GetBytes());
        }
        public void AddfHex(fHex extra)
        {
            AddByteArray(extra.GetBytes());
        }
        public void SetFromByteArray(byte[] extraBytes)
        {
            this.data = extraBytes;
        }
        public int ToNumber(int index = 0)
        {
            return BitConverter.ToInt32(data, index);
        }
        public string ToTextString(int index = 0, int? count = null)
        {
            if (count == null)
                count = (int)data.Length;
            return Encoding.UTF8.GetString(data, index, (int)count);
        }
        public override string ToString()
        {
            string retString = "";
            foreach (byte bite in data)
            {
                retString += bite.ToString("X2") + " ";
            }
            return retString;
        }
        public static fHex FromByteArrayString(string hexString,int Base = 16)
        {
            string[] hexValuesSplit = hexString.Split(' ');
            byte[] bytes = new byte[hexValuesSplit.Length];

            for (int i = 0; i < hexValuesSplit.Length; i++)
            {
                if (string.IsNullOrEmpty(hexValuesSplit[i]))
                    continue;
                bytes[i] = Convert.ToByte(hexValuesSplit[i], Base);
            }
            return new fHex(bytes);
        }

        public byte[] GetBytes()
        {
            return this.data;
        }
        public void Base64Encode() 
        {
            this.data = new fHex(Convert.ToBase64String(this.data)).GetBytes();
        }
        public void Base64Decode()
        {
            this.data = Convert.FromBase64String(ToTextString());
        }
        public Dictionary<string, string> ToDictionary()
        {
            try
            {
                if (ToTextString() == "")
                    return emptyOptions;

                Dictionary<string, string> ReturnDictionary = new Dictionary<string, string>();
                List<fHex> splitted = Split(this, ",");
                foreach (fHex fhex in splitted)
                {
                    List<fHex> oneTwo = Split(fhex, "=");
                    if (oneTwo.Count < 2)
                        return emptyOptions;
                    ReturnDictionary.Add(oneTwo[0].ToTextString().DictDecode(), oneTwo[1].ToTextString().DictDecode());
                }
                return ReturnDictionary;
            }
            catch (Exception ex)
            {
                return emptyOptions;
                //throw new Exception("Error:" + ex.Message);
            }
        }
        public static string FromDictionary(Dictionary<string, string> dictionary)
        {
            if (dictionary.Count == 0)
                return "";
            string compiledDict = "";
            foreach (string key in dictionary.Keys)
            {
                compiledDict += $"{key.DictEncode()}={dictionary[key].DictEncode()},";
            }
            compiledDict = compiledDict.Substring(0, compiledDict.Length - 1);
            return compiledDict;
        }
        public static Dictionary<string, string> GetDictionary(string textForm)
        {
            try
            {
                if (string.IsNullOrEmpty(textForm))
                    return emptyOptions;

                Dictionary<string, string> ReturnDictionary = new Dictionary<string, string>();
                List<string> splitted = textForm.Split(',').ToList();
                foreach (string split in splitted)
                {
                    string[] oneTwo = split.Split('=');
                    if (oneTwo.Length < 1)
                        return null;
                    ReturnDictionary.Add(oneTwo[0].DictDecode(), oneTwo[1].DictDecode());
                }
                return ReturnDictionary;
            }
            catch (Exception ex)
            {
                return emptyOptions;
            }
        }
        public static Dictionary<string, string> emptyOptions => new Dictionary<string, string>();

        public static string Base64Encode(string text) 
        {
            return Convert.ToBase64String(new fHex(text).GetBytes());
        }
        public static string Base64Decode(string Base64)
        {
            return new fHex(Convert.FromBase64String(Base64)).ToTextString();
        }

        public static List<fHex> Split(fHex Input, string signature)
        {
            fHex Signature = new fHex(signature);

            string strSig = Signature.ToString();
            string strInput = Input.ToString();
            List<fHex> outputList = new List<fHex>();
            if (strInput.Contains(strSig))
            {
                var inputList = Input.GetBytes().ToList();
                var signatureList = Signature.GetBytes().ToList();

                var output = SplitList(inputList, signatureList);
                foreach (byte[] bytes in output)
                {
                    outputList.Add(new fHex(bytes));
                }
            }
            else
            {
                outputList.Add(Input);
            }
            return outputList;
        }
        public static List<byte[]> SplitList(List<byte> inputList, List<byte> signature)
        {
            byte[] inputArray = inputList.ToArray();
            byte[] signatureArray = signature.ToArray();

            List<byte[]> subArrays = new List<byte[]>();

            int startIndex = 0;
            int endIndex;
            while ((endIndex = IndexOfSignature(inputArray, signatureArray, startIndex)) != -1)
            {
                int subArrayLength = endIndex - startIndex;
                byte[] subArray = new byte[subArrayLength];
                Array.Copy(inputArray, startIndex, subArray, 0, subArrayLength);
                subArrays.Add(subArray);
                startIndex = endIndex + signatureArray.Length;
            }
            // add the final subarray if the end of the original array wasn't a signature
            if (startIndex < inputArray.Length)
            {
                byte[] subArray = new byte[inputArray.Length - startIndex];
                Array.Copy(inputArray, startIndex, subArray, 0, subArray.Length);
                subArrays.Add(subArray);
            }

            return subArrays;

        }
        // helper method to find the index of a signature in an array
        public static int IndexOfSignature(byte[] array, byte[] signature, int startIndex)
        {
            int index = Array.IndexOf(array, signature[0], startIndex);
            while (index >= 0 && index <= array.Length - signature.Length)
            {
                bool match = true;
                for (int i = 0; i < signature.Length; i++)
                {
                    if (array[index + i] != signature[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return index;
                }
                index = Array.IndexOf(array, signature[0], index + 1);
            }
            return -1; // signature not found
        }

    }
    public static class fHexExtentions
    {
        public static string DictEncode(this string input)
        {
            return 
                input
                .Replace("=", "%EQUALS%")
                .Replace(",", "%COMMA%")
                ;
        }
        public static string DictDecode(this string input)
        {
            return
                input
                .Replace("%EQUALS%", "=")
                .Replace("%COMMA%", ",")
                ;
        }
    }
}