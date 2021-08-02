/* USAGE:
 * SharedObject so = SharedObjectParser.Parse(so path);
 * so.Get("key").key_type; (key_type is int/bool/string);
 * https://github.com/TurboMask/Flash-SharedObject-Parser
 */

using System;
using System.Collections.Generic;
using System.IO;

public class SharedObject
{
    public List<SOValue> values;

    public SharedObject()
    {
        values = new List<SOValue>();
    }

    public SOValue Get(string keyword)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i].key == keyword)
            {
                return values[i];
            }
        }
        return new SOValue();   //Return UNDEFINED
    }
}

public struct SOValue
{
    public string key;
    public byte type;
    public string string_val;
    public bool bool_val;
    public int int_val;
    public double double_val;
    public bool[] array_val;
}

class SOReader
{
    public byte[] file_data;
    public int file_size;
    public int pos;

    public SOReader(string filename)
    {
        file_data = File.ReadAllBytes(filename);
        file_size = 0;
        pos = 0;
    }

    public byte Read8()
    {
        return file_data[pos++];
    }

    public UInt16 Read16()
    {
        UInt16 val = file_data[pos++];
        val = (UInt16)((val << 8) | file_data[pos++]);
        return val;
    }

    public UInt32 Read32()
    {
        UInt32 val = 0;
        for (int i = 0; i < 4; i++)
        {
            val = (UInt32)((val << 8) | file_data[pos++]);
        }
        return val;
    }

    public Int32 ReadCompressedInt()
    {
        Int32 val = 0;
        byte part = 0;
        bool finished = true;
        Int32 data_bytes = 0;
        for (int i = 0; i < 3; i++)
        {
            part = Read8();
            finished = (part & 0x80) == 0;
            val = (val << 7);
            val |= (Int32)(part & 0b01111111);
            data_bytes += 7;
            if (finished)
            {
                break;
            }
        }
        if (!finished)
        {
            part = Read8();
            val = (val << 8);
            val |= (Int32)part;
            data_bytes += 8;
        }
        //Check if number is negative. Only numbers with 29 data bytes can be negative.
        if (val >> (data_bytes - 1) == 1 && data_bytes == 29)
        {
            val = (Int32)(-(~(val | (0xFFFFFFFF << data_bytes)) + 1));
        }
        return val;
    }

    public double ReadDouble()
    {
        byte[] double_raw = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            double_raw[i] = file_data[pos + 7 - i];
        }
        pos += 8;
        double val = BitConverter.ToDouble(double_raw, 0);
        return val;
    }

    public string ReadString(int length)
    {
        string val = System.Text.Encoding.UTF8.GetString(file_data, pos, length);
        pos += length;
        return val;
    }
}

struct SOHeader
{
    public UInt16 padding1;
    public UInt32 file_size;
    public string so_type;
    public UInt16 padding2;
    public UInt32 padding3;
}

struct SOTypes
{
    public const byte TYPE_NUMBER = 0x00;
    public const byte TYPE_BOOL = 0x01;
    public const byte TYPE_STRING = 0x02;
    public const byte TYPE_OBJECT = 0x03;
    public const byte TYPE_NULL = 0x05;
    public const byte TYPE_UNDEFINED = 0x06;
    public const byte TYPE_ARRAY = 0x08;
    public const byte TYPE_DATE = 0x0b;
    public const byte TYPE_XML = 0x0f;

    /*
    Undefined - 0x00
    Null - 0x01
    Boolean False - 0x02
    Boolean True - 0x03
    Integer - 0x04 (expandable 8+ bit integer)
    Double - 0x05 (Encoded as IEEE 64-bit double-precision floating point number)
    String - 0x06 (expandable 8+ bit integer string length with a UTF-8 string)
    XML - 0x07 (expandable 8+ bit integer string length and/or flags with a UTF-8 string)
    Date - 0x08 (expandable 8+ bit integer flags with an IEEE 64-bit double-precision floating point UTC offset time)
    Array - 0x09 (expandable 8+ bit integer entry count and/or flags with optional expandable 8+ bit integer name lengths with a UTF-8 names)
    Object - 0x0A (expandable 8+ bit integer entry count and/or flags with optional expandable 8+ bit integer name lengths with a UTF-8 names)
    XML End - 0x0B (expandable 8+ bit integer flags)
    ByteArray - 0x0C (expandable 8+ bit integer flags with optional 8 bit byte length)
    VectorInt - 0x0D
    VectorUInt - 0x0E
    VectorDouble - 0x0F
    VectorObject - 0x10
    Dictionary - 0x11
    The first 4 types are not followed by any data(Booleans have two types in AMF3).
    */
}

public class SharedObjectParser
{
    public static SharedObject Parse(string filename, SharedObject so = null)
    {
        if (so == null)
        {
            so = new SharedObject();
        }
        if (!File.Exists(filename))
        {
            Console.WriteLine("SharedObject " + filename + " doesn't exist.");
            return so;
        }
        SOReader file = new SOReader(filename);
        List<string> string_table = new List<string>();

        //Read header
        SOHeader header = new SOHeader();
        header.padding1 = file.Read16();
        header.file_size = file.Read32();
        file.file_size = (int)header.file_size + 6;
        header.so_type = file.ReadString(4);
        header.padding2 = file.Read16();
        header.padding3 = file.Read32();
        Console.WriteLine("Data size: " + header.file_size);

        //Read SO name and othe rparameters
        UInt16 so_name_length = file.Read16();
        string so_name = file.ReadString(so_name_length);
        //string_table.Add(so_name);
        UInt32 padding4 = file.Read32();
        Console.WriteLine("SO name: " + so_name);
        Console.WriteLine("SO type: " + header.so_type);

        while (file.pos < file.file_size)
        {
            SOValue so_value = new SOValue();

            // Read parameter name. Name length is encoded into 7 bits, 8th bit is flag if name is inline or indexed.
            UInt16 length_int = file.Read16();
            so_value.key = file.ReadString((int)length_int);
            Console.WriteLine(so_value.key);

            // Read parameter value. First byte is value type.
            so_value.type = file.Read8();
            if (so_value.type == SOTypes.TYPE_NULL)
            {
                Console.WriteLine("\tNULL");
            }
            else if (so_value.type == SOTypes.TYPE_NUMBER)
            {
                so_value.int_val = (int)file.ReadDouble();
                Console.WriteLine("\t" + so_value.int_val);
            }
            else if (so_value.type == SOTypes.TYPE_BOOL)
            {
                if (file.Read8() == 1)
                {
                    so_value.bool_val = true;
                    Console.WriteLine("\tTrue");
                }
                else
                {
                    so_value.bool_val = false;
                    Console.WriteLine("\tFalse");
                }
            }
            else if (so_value.type == SOTypes.TYPE_ARRAY)
            {
                UInt32 arr_length = file.Read32();
                bool[] arr = new bool[arr_length];

                for (var i = 0; i < arr_length; i++)
                {
                    UInt16 name_length = file.Read16();
                    string name = file.ReadString(name_length);

                    so_value.type = file.Read8();
                    if (so_value.type == SOTypes.TYPE_BOOL)
                    {
                        if (file.Read8() == 1)
                        {
                            so_value.bool_val = true;
                            Console.WriteLine("\tTrue");
                        }
                        else
                        {
                            so_value.bool_val = false;
                            Console.WriteLine("\tFalse");
                        }
                    }

                    arr[i] = so_value.bool_val;
                    so_value.array_val = arr;
                    so.values.Add(so_value);
                }

                file.Read16();
                file.Read8();
            }
            else
            {
                Console.WriteLine("Type not implemented yet: " + so_value.type);
                //Move read position to next item
                while (file.pos < file.file_size)
                {
                    byte next_byte = file.Read8();
                    if (next_byte == 0)
                    {
                        --file.pos;
                        break;
                    }
                }
            }
            so.values.Add(so_value);
            if (file.pos < file.file_size)
            {
                file.Read8();   //Padding
            }
        }
        return so;
    }
}