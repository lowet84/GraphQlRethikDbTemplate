﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Schema.Types;

namespace GraphQlRethinkDbTemplate.Schema.Model
{
    public class Author : NodeBase<Author>
    {
        public Author(string fistName, string lastName)
        {
            Name = new Name(fistName, lastName);
        }

        public Name Name { get; set; }
    }

    public class Name
    {
        public Name(string fistName, string lastName)
        {
            FistName = fistName;
            LastName = lastName;
        }

        public string FistName { get; }
        public string LastName { get; }
    }
}
