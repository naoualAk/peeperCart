using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele.AlliedVision
{
    using System;
    using System.Drawing;

    /// <summary>
    /// EventArgs class that will contain a single image
    /// </summary>
    public class FrameEventArgs : EventArgs
    {
        /// <summary>
        /// The Image (data)
        /// </summary>
        private Image m_Image = null;

        /// <summary>
        /// The Exception (data)
        /// </summary>
        private Exception m_Exception = null;

        /// <summary>
        /// Initializes a new instance of the FrameEventArgs class. 
        /// </summary>
        /// <param name="image">The Image to transfer</param>
        public FrameEventArgs(Image image)
        {
            if (null == image)
            {
                throw new ArgumentNullException("image");
            }

            m_Image = image;
        }

        /// <summary>
        /// Initializes a new instance of the FrameEventArgs class. 
        /// </summary>
        /// <param name="exception">The Exception to transfer</param>
        public FrameEventArgs(Exception exception)
        {
            if (null == exception)
            {
                throw new ArgumentNullException("exception");
            }

            m_Exception = exception;
        }

        /// <summary>
        /// Gets the image 
        /// </summary>
        public Image Image
        {
            get
            {
                return m_Image;
            }
        }

        /// <summary>
        /// Gets the Exception
        /// </summary>
        public Exception Exception
        {
            get
            {
                return m_Exception;
            }
        }
    }
}
