using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.DATA/*.MetaData*/
{
    public class CoursMetaData
    {
        [Required]
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required]
        [Display(Name = "Course Description")]
        public string CourseDescription { get; set; }
        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(CoursMetaData))]
    public partial class Cours
    {

    }

    public class CourseCompletionMetaData
    {
        [Required]
        [Display(Name = "Course Completion ID")]
        public int CourseCompletionId { get; set; }
        [Required]
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }
        [Required]
        [Display(Name = "Date Completed")]
        public System.DateTime DateCompleted { get; set; }
        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }
    }
    [MetadataType(typeof(CourseCompletionMetaData))]
    public partial class CourseCompletion
    {

    }

    public class EntryMetaData
    {
        [Required]
        [Display(Name = "Entry ID")]
        public int EntryId { get; set; }
        [Required]
        [Display(Name = "User Entry ID")]
        public string UserEntryId { get; set; }
        [Required]
        [Display(Name = "Metal Type")]
        public int MetalType { get; set; }
        [Display(Name = "Place Of Purchase")]
        public string PlaceOfPurchase { get; set; }
        [Display(Name = "Date Of Purchase")]
        public Nullable<System.DateTime> DateOfPurchase { get; set; }
        [Required]
        [Display(Name = "Amount Purchased")]
        public decimal AmountOfPurchase { get; set; }
    }

    [MetadataType(typeof(EntryMetaData))]
    public partial class Entry
    {

    }

    public class LessonMetaData
    {
        [Required]
        [Display(Name = "Lesson ID")]
        public int LessonId { get; set; }

        [Required]
        [Display(Name = "Lesson Title")]
        public string LessonTitle { get; set; }

        [Required]
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }


        public string Introduction { get; set; }

        [Display(Name = "Video URl")]
        public string VideoUrl { get; set; }

        [Display(Name = "PDF File Name")]
        public string PdfFileName { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(LessonMetaData))]
    public partial class Lesson
    {

    }

    public partial class LessonViewMetaData
    {
        [Required]
        [Display(Name = "Lesson View ID")]
        public int LessonViewId { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Date Viewed")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public System.DateTime DateViewed { get; set; }

        [Required]
        [Display(Name = "Lesson ID")]
        public int LessonId { get; set; }
    }

    [MetadataType(typeof(LessonViewMetaData))]
    public partial class LessonView
    {

    }

    public class MetalMetaData
    {
        [Required]
        [Display(Name = "Metal ID")]
        public int MetalId { get; set; }
        [Required]
        [Display(Name = "Metal Name")]
        public string Metal_Name { get; set; }
    }

    [MetadataType(typeof(MetalMetaData))]
    public partial class Metal
    {

    }

    public class UserDetailMetaData
    {
        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
    [MetadataType(typeof(UserDetailMetaData))]
    public partial class UserDetail
    {
        public string FullName { get { return FirstName + " " + LastName; } }
    }



}
