using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1
{
    public class AssignButtonContent
    {
        public string buttonContent { get; set; }
        public bool inAssignDict { get; set; }
        public String backgroundColor { get; set; }

        public AssignButtonContent()
        {
            inAssignDict = false;
            buttonContent = "";
            backgroundColor = "LightGray";
        }
    }
}
