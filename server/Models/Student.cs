﻿using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class Student
    {
        public Student()
        {
            CourseResults = new HashSet<CourseResult>();
        }

        public string Id { get; set; }
        public string FullName { get; set; }

        public virtual ICollection<CourseResult> CourseResults { get; set; }
    }
}
