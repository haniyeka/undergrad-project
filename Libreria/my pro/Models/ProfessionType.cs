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
    
    public partial class ProfessionType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProfessionType()
        {
            this.BookMansProfessions = new HashSet<BookMansProfession>();
        }
    
        public long PK_ProfessionTypeID { get; set; }
        public string ProfrssionTypeName { get; set; }
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookMansProfession> BookMansProfessions { get; set; }
    }
}
