﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Empower.Mvc.Models
{
    public class ActorListViewModel
    {
        public IList<Empower.NHibernate.Entities.Actor> Actors { get; set; }
    }
}
