using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapiProject.Services.Interfaces {

    public interface ICourseService {

        bool CourseExists(int id);
    }
}