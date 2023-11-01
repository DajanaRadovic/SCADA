using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters mdbRead = this.CommandParameters as ModbusReadCommandParameters;
            byte[] mdbRequest = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbRead.TransactionId)), 0, mdbRequest, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbRead.ProtocolId)), 0, mdbRequest, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbRead.Length)), 0, mdbRequest, 4, 2);
            mdbRequest[6] = mdbRead.UnitId;
            mdbRequest[7] = mdbRead.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbRead.StartAddress)), 0, mdbRequest, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbRead.Quantity)), 0, mdbRequest, 10, 2);
            return mdbRequest;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters mdbRead = this.CommandParameters as ModbusReadCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCount = response[8];
            byte[] bajtovi = new byte[byteCount];
            Array.Copy(response, 9, bajtovi, 0, byteCount);

            ushort adresa = 0;

            for (int i = 0; i < byteCount; i++)
            {
                byte[] vrijednost = new byte[2];
                vrijednost[0] = bajtovi[i + 1];
                vrijednost[1] = bajtovi[i];

                ushort value = BitConverter.ToUInt16(vrijednost, 0);
                dict.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)(mdbRead.StartAddress + adresa)), value);

                adresa++;
                i++;
            }

            return dict;
        }
    }
}