using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AesSource
{
    public class AesController
    {

        public AesController()
        {

        }



        public GenericResult<string> Encrypt(string targetText, string keyText)
        {
            var genericResult = new GenericResult<string>();
            byte[] targetTextBytes = Encoding.ASCII.GetBytes(targetText);
            byte[] targetKeyBytes = Encoding.ASCII.GetBytes(keyText);
            fillEmptyBytes(ref targetKeyBytes);
            fillEmptyBytes(ref targetTextBytes);
            byte[] targetBlock = new byte[16];
            byte[] keyBlock = new byte[16];
            Array.Copy(targetTextBytes, 0, targetBlock, 0, 16);
            Array.Copy(targetKeyBytes, 0, keyBlock, 0, 16);

            addRoundKey(ref targetBlock, keyBlock);
            for (int roundCounter = 0; roundCounter < 9; roundCounter++)
            {
                subsituteBytes(ref targetBlock);
                shiftRows(ref targetBlock, Direction.Left);
                mixColumns(ref targetBlock);
                getNextRoundKey(ref keyBlock, roundCounter);
                addRoundKey(ref targetBlock, keyBlock);
            }

            subsituteBytes(ref targetBlock);
            shiftRows(ref targetBlock, Direction.Left);
            getNextRoundKey(ref keyBlock, 9);
            addRoundKey(ref targetBlock, keyBlock);
            genericResult.ResultValue= Convert.ToBase64String(targetBlock);            
            return genericResult;
        }

        public GenericResult<string> Decrypt(string targetText, string keyText)
        {

            var genericResult = new GenericResult<string>();
            byte[] targetTextBytes = Convert.FromBase64String(targetText);                        
            byte[] targetKeyBytes = Encoding.ASCII.GetBytes(keyText);
            fillEmptyBytes(ref targetKeyBytes);
            fillEmptyBytes(ref targetTextBytes);
            byte[] targetBlock = new byte[16];
            byte[] keyBlock = new byte[16];
            Array.Copy(targetTextBytes, 0, targetBlock, 0, 16);
            Array.Copy(targetKeyBytes, 0, keyBlock, 0, 16);
            var initialKey = new byte[16];
            Array.Copy(keyBlock, initialKey,16);
            var allRoundKeyList = getAllRoundKeyList(keyBlock);

            addRoundKey(ref targetBlock,allRoundKeyList[9]);
            shiftRows(ref targetBlock, Direction.Right);
            subsituteBytes(ref targetBlock, Constants.InverseSBox);

            for (int roundCounter = 8; roundCounter >= 0; roundCounter--)
            {
                addRoundKey(ref targetBlock, allRoundKeyList[roundCounter]);
                inverseMixColumn(ref targetBlock);
                shiftRows(ref targetBlock, Direction.Right);
                subsituteBytes(ref targetBlock,Constants.InverseSBox);                
            }

            addRoundKey(ref targetBlock, initialKey);

            genericResult.ResultValue = Encoding.ASCII.GetString(targetBlock);
            return genericResult;
        }

        private List<byte[]> getAllRoundKeyList(byte[] keyBlock)
        {
            var roundKeyList = new List<byte[]>();
            for (int i = 0; i < 10; i++)
            {
                var tempKey = new byte[16];
                getNextRoundKey(ref keyBlock, i);
                Array.Copy(keyBlock, tempKey, 16);
                roundKeyList.Add(tempKey);
            }
            return roundKeyList;
        }
        private void getNextRoundKey(ref byte[] keyBlock,int roundNo)
        {
            byte[] keyword = new byte[4];
            Array.Copy(keyBlock, 12, keyword, 0, 4);
            keywordTransform(ref keyword, roundNo);
            for (int k = 0; k < 4; ++k)
                keyBlock[k] ^= keyword[k];
            for (int i = 0; i < 12; ++i)
                keyBlock[i + 4] ^= keyBlock[i];
        }
        private void keywordTransform(ref byte[] keyword, int roundno)
        { //round no starts from 0, ends at 9
            byte buf = keyword[0];
            keyword[0] = (byte)(Constants.SBox[keyword[1]] ^ Constants.RoundCoefficient[roundno]);
            keyword[1] = Constants.SBox[keyword[2]];
            keyword[2] = Constants.SBox[keyword[3]];
            keyword[3] = Constants.SBox[buf];
        }
        private void addRoundKey(ref byte[] targetBlock,byte[] keyBlock)
        {
            for (int i = 0; i < 16; i++)
            {
                targetBlock[i] =(byte)(targetBlock[i] ^ keyBlock[i]);
            }
        }
        private void keyTransform(byte[] key, int roundno)
        {
            
        }
        private byte gMul(byte a, byte b)
        { // Galois Field (256) Multiplication of two Bytes
            byte p = 0;

            for (int counter = 0; counter < 8; counter++)
            {
                if ((b & 1) != 0)
                {
                    p ^= a;
                }

                bool hi_bit_set = (a & 0x80) != 0;
                a <<= 1;
                if (hi_bit_set)
                {
                    a ^= 0x1B; /* x^8 + x^4 + x^3 + x + 1 */
                }
                b >>= 1;
            }

            return p;
        }
        private byte galoisDefaultMult(byte val, byte mult)
        {
            int buf = val << 3;
            if (mult != 0x0E)
                buf ^= val;
            if (mult > 0x0C)
                buf ^= val << 2;
            if ((mult & 0x02) > 0)
                buf ^= val << 1;
            byte xorval = Constants.QuickXORTable[buf >> 8];
            return xorval == 0 ? (byte)buf : (byte)(buf ^ xorval);
        }
        private void mixColumns(ref byte[] targetBlock)
        {
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
            {
                var targetColumn = getColumnFromArray(targetBlock, columnIndex);
                var tempColumn = new byte[4];
                tempColumn[0] = (byte)(gMul(0x02, targetColumn[0]) ^ gMul(0x03, targetColumn[1]) ^ targetColumn[2] ^ targetColumn[3]);
                tempColumn[1] = (byte)(targetColumn[0] ^ gMul(0x02, targetColumn[1]) ^ gMul(0x03, targetColumn[2]) ^ targetColumn[3]);
                tempColumn[2] = (byte)(targetColumn[0] ^ targetColumn[1] ^ gMul(0x02, targetColumn[2]) ^ gMul(0x03, targetColumn[3]));
                tempColumn[3] = (byte)(gMul(0x03, targetColumn[0]) ^ targetColumn[1] ^ targetColumn[2] ^ gMul(0x02, targetColumn[3]));
                setColumnToArray(ref targetBlock, tempColumn, columnIndex);
            }
        }        
        private void inverseMixColumn(ref byte[] text)
        {
            byte[] temp = new byte[4];
            int p, p2;
            for (int i = 0; i < 4; ++i)
            {
                p = i * 4;
                for (int j = 0; j < 4; ++j)
                {
                    p2 = 3 - j;
                    temp[j] = (byte)(galoisDefaultMult(text[p], Constants.InverseMixColumnMatrixElementTable[p2]) ^ galoisDefaultMult(text[p + 1], Constants.InverseMixColumnMatrixElementTable[p2 + 1])
                      ^ galoisDefaultMult(text[p + 2], Constants.InverseMixColumnMatrixElementTable[p2 + 2]) ^ galoisDefaultMult(text[p + 3], Constants.InverseMixColumnMatrixElementTable[p2 + 3]));
                }
                Array.Copy(temp, 0, text, p, 4);
            }
        }
        private void subsituteBytes(ref byte[] targetBlock)
        {
            for (var i = 0; i < 16; i++)
            {
                targetBlock[i] = Constants.SBox[targetBlock[i]];
            }
        }
        private void subsituteBytes(ref byte[] targetBlock,byte[] sBox)
        {
            for (var i = 0; i < 16; i++)
            {
                targetBlock[i] = sBox[targetBlock[i]];
            }
        }
        private void shiftRows(ref byte[] targetBlock, Direction direction)
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                var targetRow = getRowFromArray(targetBlock, rowIndex);
                var tempRow = new Byte[4];
                Array.Copy(targetRow, tempRow, 4);
                for (int oldIndex = 0; oldIndex < 4; oldIndex++)
                {
                    var newIndex = direction == Direction.Right ?
                        (oldIndex + rowIndex) % 4 :
                        (oldIndex - rowIndex) % 4;
                    newIndex = newIndex < 0 ? newIndex + 4 : newIndex;
                    tempRow[newIndex] = targetRow[oldIndex];
                }
                setRowToArray(ref targetBlock, tempRow, rowIndex);
            }
        }
        private void setColumnToArray(ref byte[] targetBlock, byte[] targetColumn,int columnIndex)
        {            
            var initialIndex = columnIndex * 4;
            targetBlock[initialIndex] = targetColumn[0];
            targetBlock[initialIndex + 1] = targetColumn[1];
            targetBlock[initialIndex + 2] = targetColumn[2];
            targetBlock[initialIndex + 3] = targetColumn[3];
        }
        private byte[] getColumnFromArray(byte[] targetBlock,int columnIndex)
        {
            var targetColumn = new byte[4];
            var initialIndex = columnIndex*4;
            targetColumn[0] = targetBlock[initialIndex];
            targetColumn[1] = targetBlock[initialIndex+1];
            targetColumn[2] = targetBlock[initialIndex+2];
            targetColumn[3] = targetBlock[initialIndex+3];
            return targetColumn;
        }
        private void setRowToArray(ref byte[] targetBlock, byte[] targetRow, int rowIndex)
        {
            var initialIndex = rowIndex;
            targetBlock[initialIndex] = targetRow[0];
            targetBlock[initialIndex + 4] = targetRow[1];
            targetBlock[initialIndex + 8] = targetRow[2];
            targetBlock[initialIndex + 12] = targetRow[3];
        }
        private byte[] getRowFromArray(byte[] targetBlock, int rowIndex)
        {
            var targetRow = new byte[4];
            var initialIndex = rowIndex;
            targetRow[0] = targetBlock[initialIndex];
            targetRow[1] = targetBlock[initialIndex + 4];
            targetRow[2] = targetBlock[initialIndex + 8];
            targetRow[3] = targetBlock[initialIndex + 12];
            return targetRow;
        }
        private void fillEmptyBytes(ref byte[] byteArray)
        {
            if (byteArray.Length < 16)
            {
                var oldlength = byteArray.Length;
                Array.Resize(ref byteArray, 16);
                for (int i = oldlength; i < 16; i++)
                {
                    byteArray[i] = 0xFF;
                }

            }
        }

    }
}
