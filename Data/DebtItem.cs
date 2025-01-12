﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Todo.Data
{
    public class DebtItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please provide the task name.")]
        public string TaskName { get; set; }

        public string SourceofDebt { get; set; }

        public string DebtAmount { get; set; }

        //public string Tag { get; set; }
        public bool IsDone { get; set; }

   


        [Required(ErrorMessage = "Please provide a due date.")]
        public DateTime DueDate { get; set; } = DateTime.Today;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
