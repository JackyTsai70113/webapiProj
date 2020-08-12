using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapiProject.Models;

namespace webapiProject.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase {
        private readonly ContosoUniversityContext _context;

        public CourseController(ContosoUniversityContext context) {
            _context = context;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse() {
            return await _context.Course.ToListAsync();
        }

        // GET: api/Course/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Course>> GetCourse(int id) {
            var course = await _context.Course.FindAsync(id);

            if (course == null) {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Course/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutCourse(int id, Course course) {
            if (id != course.CourseId) {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!CourseExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Course
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Course>> PostCourse(Course course) {
            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Course/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Course>> DeleteCourse(int id) {
            var course = await _context.Course.FindAsync(id);
            if (course == null) {
                return NotFound();
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }

        // GET: api/Course/Student
        [HttpGet("student")]
        public async Task<ActionResult<List<VwCourseStudents>>> GetVwCourseStudents() {
            string sql =
                $"SELECT TOP (1000) " +
                $"  [DepartmentID], [DepartmentName], [CourseID], " +
                $"  [CourseTitle], [StudentID], [StudentName] " +
                $"FROM [dbo].[vwCourseStudents]";

            var vwCourseStudents = _context
                .VwCourseStudents
                .FromSqlRaw(sql).ToList();

            if (!vwCourseStudents.Any()) {
                return NoContent();
            }

            return vwCourseStudents;
        }

        // GET: api/Course/Student/Count
        [HttpGet("student/count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<List<VwCourseStudentCount>>> GetVwCourseStudentCount() {
            string sql =
                $"SELECT TOP (1000) " +
                $"  [DepartmentID], [Name], [CourseID], " +
                $"  [Title], [StudentCount] " +
                $"FROM [dbo].[vwCourseStudentCount]";

            var vwCourseStudentCount = _context
                .VwCourseStudentCount
                .FromSqlRaw(sql).ToList();

            if (!vwCourseStudentCount.Any()) {
                return NoContent();
            }

            return vwCourseStudentCount;
        }

        private bool CourseExists(int id) {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}