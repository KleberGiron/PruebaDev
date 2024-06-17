using Api_Covid.Datos;
using Api_Covid.Models;
using Api_Covid.Models.Dto;
using Api_Covid.Repository.IRepository;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Api_Covid.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidController : ControllerBase
    {
        private readonly ILogger<CovidController> _logger;
        private readonly IEmpleado _empleadoRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public CovidController(ILogger<CovidController> logger, IEmpleado empleadoRepo, IMapper mapper)
        {
            _logger = logger;
            _empleadoRepo = empleadoRepo;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        //[ResponseCache(CacheProfileName = "Default30")]
        // [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetEmpleado()
        {
            var _response = new APIResponse();
            try
            {
                _logger.LogInformation("Obtener los empleados");

                // Obtener todos los empleados sin filtros
                IEnumerable<Empleado> empleadosList = await _empleadoRepo.ObtenerTodos();

                // Verificar si la lista está vacía
                if (empleadosList == null || !empleadosList.Any())
                {
                    _response.IsExitoso = false;
                    _response.ErrorMessages = new List<string> { "No se encontraron empleados." };
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Mapear los empleados a DTOs
                _response.Resultado = _mapper.Map<IEnumerable<EmpleadosDto>>(empleadosList);
                _response.statusCode = HttpStatusCode.OK;
                _response.IsExitoso = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.statusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }



        [HttpGet("{id:int}", Name = "GetEmpleado")]
        //[Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetEmpleado(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Empleado con Id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }
                var villa = await _empleadoRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<EmpleadosDto>(villa);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        //[Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearEmpleado([FromBody] EmpleadosCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _empleadoRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "El Empleado con ese Nombre ya existe!");
                    return BadRequest(ModelState);
                }
                if (await _empleadoRepo.Obtener(v => v.Apellido.ToLower() == createDto.Apellido.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "El Empleado con ese Apellido ya existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                Empleado modelo = _mapper.Map<Empleado>(createDto);

                modelo.PuestoLaboral = createDto.PuestoLaboral;
                modelo.FechaDosisVacuna = DateTime.Now;
                await _empleadoRepo.Crear(modelo);

                var _response = new APIResponse
                {
                    Resultado = modelo,
                    statusCode = HttpStatusCode.Created
                };

                return CreatedAtRoute("GetEmpleado", new { id = modelo.Id }, _response);
            }
            catch (Exception ex)
            {
                var _response = new APIResponse
                {
                    IsExitoso = false,
                    ErrorMessages = new List<string> { ex.ToString() },
                    statusCode = HttpStatusCode.InternalServerError
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeletEmpleado(int id)
        {
            var _response = new APIResponse();
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var empleado = await _empleadoRepo.Obtener(v => v.Id == id);
                if (empleado == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                await _empleadoRepo.Remover(empleado);

                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.statusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [HttpPut("{id:int}")]
        //[Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] EmpleadosUpdateDto updateDto)
        {
            var _response = new APIResponse();
            try
            {
                if (updateDto == null || id != updateDto.Id)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Empleado modelo = _mapper.Map<Empleado>(updateDto);

                await _empleadoRepo.Actualizar(modelo);
                _response.statusCode = HttpStatusCode.NoContent;
                _response.IsExitoso = true;
                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.statusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }


        [HttpPatch("{id:int}")]
        //[Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EmpleadosUpdate(int id, JsonPatchDocument<EmpleadosUpdateDto> patchDto)
        {
            var _response = new APIResponse();

            try
            {
                if (patchDto == null || id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var empleado = await _empleadoRepo.Obtener(v => v.Id == id, tracked: false);

                if (empleado == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                EmpleadosUpdateDto empleadoDto = _mapper.Map<EmpleadosUpdateDto>(empleado);

                //patchDto.ApplyTo(empleadoDto, ModelState);

                if (!ModelState.IsValid)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                Empleado modelo = _mapper.Map<Empleado>(empleadoDto);

                await _empleadoRepo.Actualizar(modelo);

                _response.statusCode = HttpStatusCode.NoContent;
                _response.IsExitoso = true;

                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                _response.statusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
    }
}
