using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapiProject.Models;
using webapiProject.Services.Interfaces;

namespace webapiProject.Services {

    public class CourseService : ICourseService {

        public CourseService(ContosoUniversityContext context) {
            Context = context;
        }

        public ContosoUniversityContext Context { get; }

        public bool CourseExists(int id) {
            return Context.Course.Any(e => e.CourseId == id);
        }
    }
}