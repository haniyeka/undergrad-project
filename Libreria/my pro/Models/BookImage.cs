//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace my_pro.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BookImage
    {
        public long PK_BookImageID { get; set; }
        public long FK_BookID { get; set; }
        public long FK_ImageID { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual Image Image { get; set; }
    }
}
