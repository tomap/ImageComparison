using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageComparisonWpfGui
{
    public class AddImageCommand : CommandBase
    {
        private readonly ImageComparisonWpfViewModel _icwvm;
        private readonly int _imageNumber;
        public AddImageCommand(ImageComparisonWpfViewModel cvm, int imageNumber)
        {
            _icwvm = cvm;
            _imageNumber = imageNumber;
        }

        public override void Execute(object parameter)
        {
            _icwvm.AddAnImage((BitmapImage) parameter, _imageNumber);
        }
    }
}

