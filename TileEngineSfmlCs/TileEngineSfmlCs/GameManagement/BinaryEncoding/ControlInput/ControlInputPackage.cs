using System;
using TileEngineSfmlCs.GameManagement.ClientControlInput;

namespace TileEngineSfmlCs.GameManagement.BinaryEncoding.ControlInput
{
    public struct ControlInputPackage : IBinaryEncodable
    {
        public MovementKey MovementKey { get; set; }
        
        public InputKeyState CtrlKeyState { get; set; }
        public InputKeyState ShiftKeyState { get; set; }
        public InputKeyState AltKeyState { get; set; }

        public int ClickedObjectId { get; set; }

        public ControlInputPackage(MovementKey movement, int clickedObjectId, InputKeyState ctrl, InputKeyState shift,
            InputKeyState alt)
        {
            MovementKey = movement;
            ClickedObjectId = clickedObjectId;
            CtrlKeyState = ctrl;
            ShiftKeyState = shift;
            AltKeyState = alt;
        }

        public ControlInputPackage(int clickedObjectId, InputKeyState ctrl, InputKeyState shift, InputKeyState alt)
        {
            MovementKey = MovementKey.None;
            ClickedObjectId = clickedObjectId;
            CtrlKeyState = ctrl;
            ShiftKeyState = shift;
            AltKeyState = alt;
        }

        public ControlInputPackage(MovementKey movement)
        {
            MovementKey = movement;
            ClickedObjectId = -1;
            CtrlKeyState = InputKeyState.KeyUp;
            ShiftKeyState = InputKeyState.KeyUp;
            AltKeyState = InputKeyState.KeyUp;
        }


        public int ByteLength => GetLength();
        public int ToByteArray(byte[] package, int index)
        {
            int pos = index;
            byte movementByte = (byte) MovementKey;

            package[pos] = movementByte;
            pos += 1;

            byte modifiersByte = 0;

            if (CtrlKeyState == InputKeyState.KeyDown)
            {
                modifiersByte |= (1 << 0);
            }
            if (ShiftKeyState == InputKeyState.KeyDown)
            {
                modifiersByte |= (1 << 1);
            }
            if (AltKeyState == InputKeyState.KeyDown)
            {
                modifiersByte |= (1 << 2);
            }

            package[pos] = modifiersByte;
            pos += 1;

            package[pos] = (byte)(ClickedObjectId >= 0 ? 1 : 0);
            pos += 1;

            if(ClickedObjectId >= 0)
            {
                byte[] clickedBytes = BitConverter.GetBytes(ClickedObjectId);
                Array.Copy(clickedBytes, 0, package, pos, clickedBytes.Length);
            }

            return ByteLength;
        }

        public void FromByteArray(byte[] data, int index)
        {
            int pos = index;
            MovementKey = (MovementKey) data[pos];
            pos++;

            byte modifiers = data[pos];
            pos++;
            if ((modifiers & (1 << 0)) != 0)
            {
                CtrlKeyState = InputKeyState.KeyDown;
            }
            else
            {
                CtrlKeyState = InputKeyState.KeyUp;
            }
            if ((modifiers & (1 << 1)) != 0)
            {
                ShiftKeyState = InputKeyState.KeyDown;
            }
            else
            {
                ShiftKeyState = InputKeyState.KeyUp;
            }
            if ((modifiers & (1 << 2)) != 0)
            {
                AltKeyState = InputKeyState.KeyDown;
            }
            else
            {
                AltKeyState = InputKeyState.KeyUp;
            }

            bool hasClickedObject = data[pos] == 1;
            pos += 1;
            if (hasClickedObject)
            {
                ClickedObjectId = BitConverter.ToInt32(data, pos);
            }
        }

        private int GetLength()
        {
            int length =
                sizeof(byte) +  // MovementKey
                sizeof(byte) +  // Modifiers
                sizeof(byte);   // HasClickedObjectId
            if (ClickedObjectId >= 0)
            {
                length += sizeof(int);
            }

            return length;
        }
    }
}