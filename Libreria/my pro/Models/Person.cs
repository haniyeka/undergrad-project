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
    
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            this.Members = new HashSet<Member>();
        }
    
        public string Fname { get; set; }
        public string Lname { get; set; }
        public long FK_UserID { get; set; }
        public long FK_DomainID { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string gender { get; set; }
        public Nullable<long> FK_ImageID { get; set; }
        public Nullable<long> FK_AddressID { get; set; }
        public long PK_PersonID { get; set; }
        public Nullable<long> PhoneNumber { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Domain Domain { get; set; }
        public virtual Image Image { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member> Members { get; set; }
        public virtual User User { get; set; }
    }
}
