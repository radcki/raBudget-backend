using System;

namespace raBudget.Common.Entities
{
    public class BaseEntity
    {
        public bool Deleted { get; private set; }
        public DateTime? DeletedDate { get; private set; }

        public void SoftDelete()
        {
            this.Deleted = true;
            DeletedDate = DateTime.Now;
        }
    }
}
