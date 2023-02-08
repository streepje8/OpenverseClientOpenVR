namespace Openverse.Audio
{
    public class AudioUtility
    {
        public const int BIAS = 0x84;
        public const int MAX = 32635;

        private static byte[] pcmToMuLawMap;
        private static short[] muLawToPcmMap;

        public static byte[] PCMToMuLawMap
        {
            get
            {
                if (pcmToMuLawMap == null)
                {
                    pcmToMuLawMap = new byte[65536];
                    for (int i = short.MinValue; i <= short.MaxValue; i++)
                        pcmToMuLawMap[(i & 0xffff)] = MuLawEncode(i);
                }

                return pcmToMuLawMap;
            }
        }

        public static short[] MuLawToPCMMap
        {
            get
            {
                if (muLawToPcmMap == null)
                {
                    muLawToPcmMap = new short[256];
                    for (byte i = 0; i < byte.MaxValue; i++)
                        muLawToPcmMap[i] = MuLawDecode(i);
                }

                return muLawToPcmMap;
            }
        }

        public static bool ZeroTrap
        {
            get { return (PCMToMuLawMap[33000] != 0); }
            set
            {
                byte val = (byte)(value ? 2 : 0);
                for (int i = 32768; i <= 33924; i++)
                    PCMToMuLawMap[i] = val;
            }
        }

        public static byte[] Compress(byte[] data)
        {
            if (!ZeroTrap) ZeroTrap = true;
            byte[] muLawCompressed = MuLawEncode(data);
            return muLawCompressed;
        }

        public static byte[] DeCompress(byte[] compressed)
        {
            byte[] muLawDeCompressed = MuLawDecode(compressed);
            return muLawDeCompressed;
        }

        //Code from https://www.codeproject.com/Articles/14237/Using-the-G711-standard
        public static byte[] MuLawEncode(byte[] data)
        {
            int size = data.Length / 2;
            byte[] encoded = new byte[size];
            for (int i = 0; i < size; i++)
                encoded[i] = PCMToMuLawMap[(data[2 * i + 1] << 8) | data[2 * i]];
            return encoded;
        }

        public static byte[] MuLawDecode(byte[] data)
        {
            int size = data.Length;
            byte[] decoded = new byte[size * 2];
            for (int i = 0; i < size; i++)
            {
                //First byte is the less significant byte
                decoded[2 * i] = (byte)(MuLawToPCMMap[data[i]] & 0xff);
                //Second byte is the more significant byte
                decoded[2 * i + 1] = (byte)(MuLawToPCMMap[data[i]] >> 8);
            }

            return decoded;
        }

        private static byte MuLawEncode(int pcm) //16-bit
        {
            //Get the sign bit. Shift it for later 
            //use without further modification
            int sign = (pcm & 0x8000) >> 8;
            //If the number is negative, make it 
            //positive (now it's a magnitude)
            if (sign != 0)
                pcm = -pcm;
            //The magnitude must be less than 32635 to avoid overflow
            if (pcm > MAX) pcm = MAX;
            //Add 132 to guarantee a 1 in 
            //the eight bits after the sign bit
            pcm += BIAS;

            /* Finding the "exponent"
            * Bits:
            * 1 2 3 4 5 6 7 8 9 A B C D E F G
            * S 7 6 5 4 3 2 1 0 . . . . . . .
            * We want to find where the first 1 after the sign bit is.
            * We take the corresponding value from
            * the second row as the exponent value.
            * (i.e. if first 1 at position 7 -> exponent = 2) */
            int exponent = 7;
            //Move to the right and decrement exponent until we hit the 1
            for (int expMask = 0x4000;
                 (pcm & expMask) == 0;
                 exponent--, expMask >>= 1)
            {
            }

            /* The last part - the "mantissa"
            * We need to take the four bits after the 1 we just found.
            * To get it, we shift 0x0f :
            * 1 2 3 4 5 6 7 8 9 A B C D E F G
            * S 0 0 0 0 0 1 . . . . . . . . . (meaning exponent is 2)
            * . . . . . . . . . . . . 1 1 1 1
            * We shift it 5 times for an exponent of two, meaning
            * we will shift our four bits (exponent + 3) bits.
            * For convenience, we will actually just shift
            * the number, then and with 0x0f. */
            int mantissa = (pcm >> (exponent + 3)) & 0x0f;

            //The mu-law byte bit arrangement 
            //is SEEEMMMM (Sign, Exponent, and Mantissa.)
            byte mulaw = (byte)(sign | exponent << 4 | mantissa);

            //Last is to flip the bits
            return (byte)~mulaw;
        }

        private static short MuLawDecode(byte mulaw)
        {
            //Flip all the bits
            mulaw = (byte)~mulaw;

            //Pull out the value of the sign bit
            int sign = mulaw & 0x80;
            //Pull out and shift over the value of the exponent
            int exponent = (mulaw & 0x70) >> 4;
            //Pull out the four bits of data
            int data = mulaw & 0x0f;

            //Add on the implicit fifth bit (we know 
            //the four data bits followed a one bit)
            data |= 0x10;
            /* Add a 1 to the end of the data by 
            * shifting over and adding one. Why?
            * Mu-law is not a one-to-one function. 
            * There is a range of values that all
            * map to the same mu-law byte. 
            * Adding a one to the end essentially adds a
            * "half byte", which means that 
            * the decoding will return the value in the
            * middle of that range. Otherwise, the mu-law
            * decoding would always be
            * less than the original data. */
            data <<= 1;
            data += 1;
            /* Shift the five bits to where they need
            * to be: left (exponent + 2) places
            * Why (exponent + 2) ?
            * 1 2 3 4 5 6 7 8 9 A B C D E F G
            * . 7 6 5 4 3 2 1 0 . . . . . . . <-- starting bit (based on exponent)
            * . . . . . . . . . . 1 x x x x 1 <-- our data
            * We need to move the one under the value of the exponent,
            * which means it must move (exponent + 2) times
            */
            data <<= exponent + 2;
            //Remember, we added to the original,
            //so we need to subtract from the final
            data -= BIAS;
            //If the sign bit is 0, the number 
            //is positive. Otherwise, negative.
            return (short)(sign == 0 ? data : -data);
        }
    }
}
