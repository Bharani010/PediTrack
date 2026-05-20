using System.ComponentModel.DataAnnotations;

namespace PediTrack.Models
{
    public class DataDictionaryEntry
    {
        [Key]
        public int EntryId { get; set; }

        [Required, StringLength(100), Display(Name = "Table Name")]
        public string TableName { get; set; } = string.Empty;

        [Required, StringLength(100), Display(Name = "Column Name")]
        public string ColumnName { get; set; } = string.Empty;

        [Required, StringLength(50), Display(Name = "Data Type")]
        public string DataType { get; set; } = string.Empty;

        [StringLength(500), Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Nullable?")]
        public bool IsNullable { get; set; }

        [StringLength(200), Display(Name = "Allowed Values / Sample")]
        public string? AllowedValues { get; set; }

        [StringLength(200), Display(Name = "Business Rule")]
        public string? BusinessRule { get; set; }

        [Display(Name = "Is Primary Key?")]
        public bool IsPrimaryKey { get; set; }

        [Display(Name = "Is Foreign Key?")]
        public bool IsForeignKey { get; set; }

        [StringLength(100), Display(Name = "References Table")]
        public string? ReferencesTable { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [StringLength(100), Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }
    }
}
