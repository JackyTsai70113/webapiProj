using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapiProject.Models;

namespace webapiProject.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase {
        private readonly ContosoUniversityContext _context;

        public DepartmentController(ContosoUniversityContext context) {
            _context = context;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentAsync() {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Department/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> GetDepartmentAsync(int id) {
            var department = await _context.Department.FindAsync(id);

            if (department == null) {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Department/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutDepartmentAsync(int id, Department department) {
            if (id != department.DepartmentId) {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!DepartmentExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Department
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> PostDepartmentAsync(Department department) {
            await _context.Department.AddAsync(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Department/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Department>> DeleteDepartmentAsync(int id) {
            var department = await _context.Department.FindAsync(id);
            if (department == null) {
                return NotFound();
            }

            _context.Department.Remove(department);
            await _context.SaveChangesAsync();

            return department;
        }

        // GET: api/Department/Course/Count
        [HttpGet("course/count")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<IEnumerable<VwDepartmentCourseCount>> GetDepartmentCourseCount() {
            string sql =
                $"SELECT TOP (1000) " +
                $"  [DepartmentID], [Name], [CourseCount]" +
                $"FROM [dbo].[vwDepartmentCourseCount]";

            var vwDepartmentCourseCount = _context
                .VwDepartmentCourseCount
                .FromSqlRaw(sql).ToArray();

            if (!vwDepartmentCourseCount.Any()) {
                return NoContent();
            }

            return vwDepartmentCourseCount;
        }

        private bool DepartmentExists(int id) {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}