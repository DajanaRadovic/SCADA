using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters mdbWriteCommParams = this.CommandParameters as ModbusWriteCommandParameters;
            byte[] mdbRequest = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbWriteCommParams.TransactionId)), 0, mdbRequest, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbWriteCommParams.ProtocolId)), 0, mdbRequest, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbWriteCommParams.Length)), 0, mdbRequest, 4, 2);
            mdbRequest[6] = mdbWriteCommParams.UnitId;
            mdbRequest[7] = mdbWriteCommParams.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbWriteCommParams.OutputAddress)), 0, mdbRequest, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdbWriteCommParams.Value)), 0, mdbRequest, 10, 2);
            return mdbRequest;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusWriteCommandParameters mdbWriteCommParams = this.CommandParameters as ModbusWriteCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            byte[] vrijednost = new byte[2];
            vrijednost[0] = response[11];
            vrijednost[1] = response[10];

            ushort value = BitConverter.ToUInt16(vrijednost, 0);

            dict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, (ushort)mdbWriteCommParams.OutputAddress), value);
            return dict;

        }
    }
}