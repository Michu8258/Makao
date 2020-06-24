using System;
using System.Collections.Generic;
using System.Text;

namespace MakaoEngine
{
    public class UpdateGameStatusEventArgs : EventArgs
    {
        public bool MoveAccepted { get; set; }
    }
}
