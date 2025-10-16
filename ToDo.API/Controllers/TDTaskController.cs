//Controller for TDTask Entity


using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToDo.DataAccess.Repositories.IRepository;
using ToDo.Models.DTOs;
using ToDo.Models.Models;

namespace ToDo.API.Controllers
{

    public class TDTaskController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly APIResponse _response;

        public TDTaskController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new APIResponse();
        }


        //Endpoint gets all TDTasks
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllTDTasks()
        {
            IEnumerable<TDTask> tasks = await _unitOfWork.TDTask.GetAll();
            _response.Result = tasks;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        //Endpoint gets specific TDTask
        [HttpGet("{id:int}", Name = "GetSpecificTask")]
        public async Task<ActionResult<APIResponse>> GetSpecificTDTask(int id)
        {
            if(id==0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = ["ID cannot be null"];
                return BadRequest(_response);
            }

            TDTask task = await _unitOfWork.TDTask.Get(u => u.Id == id);
            if(task==null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = task;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        //Endpoint returns TDTasks depending on statuses based on ExpirationDate: nextday,currentweek, today (default)
        [HttpGet("incoming/{period}", Name = "GetIncomingTasks")]
        public async Task<ActionResult<APIResponse>> GetIncomingTDTasks(string period)
        {
            if(string.IsNullOrEmpty(period))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Period cannot be null"];
                return BadRequest(_response);
            }
            var now = DateTime.UtcNow;
            IEnumerable<TDTask> tasks = new List<TDTask>();

            switch (period.ToLower())
            {
                case "nextday":
                    tasks = await _unitOfWork.TDTask.GetAll(t => t.ExpirationDate.Date == now.Date.AddDays(1));
                    break;

                case "currentweek":
                    var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                    var endOfWeek = startOfWeek.AddDays(6);
                    tasks = await _unitOfWork.TDTask.GetAll(t =>
                        t.ExpirationDate.Date >= startOfWeek &&
                        t.ExpirationDate.Date <= endOfWeek);
                    break;

                case "today":
                default:
                    tasks = await _unitOfWork.TDTask.GetAll(t => t.ExpirationDate.Date == now.Date);
                    break;
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = tasks;

            return Ok(_response);
        }


        //Endpoint creates or updates TDTask Entity(two properties skipped because of
        //endpoints that update those values,
        //function elligable for splitting
        [HttpPost("upsert", Name = "UpsertTDTask")]
        public async Task<ActionResult<APIResponse>> UpsertTDTask([FromBody] TDTaskDTO tDTaskDTO)
        {
            if (tDTaskDTO == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            TDTask newTask = _mapper.Map<TDTask>(tDTaskDTO);
            if (newTask.Id == 0)
            {
                await _unitOfWork.TDTask.Add(newTask);
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = newTask;
            }
            else
            {
                _unitOfWork.TDTask.Update(newTask);
                _response.StatusCode = HttpStatusCode.NoContent;
               
            }
                
            await _unitOfWork.Save();
           

            return Ok(_response);
        }

        //Endpoint updates CompletionPercantage of TDTask
        [HttpPatch("update-percentage",Name = "UpdatePercentage")]
        public async Task<ActionResult<APIResponse>> UpdateTDTaskPercentage([FromBody] UpdatePercentageDTO updatePercentageDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(_response);
                }

                var task = await _unitOfWork.TDTask.Get(u => u.Id == updatePercentageDTO.Id);
                if (task == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }

                task.CompletionPercentage = updatePercentageDTO.Percentage;
                if (task.CompletionPercentage == 100)
                    task.Status = "Done";
                else
                    task.Status = "InProgress";
                    _unitOfWork.TDTask.Update(task);
                await _unitOfWork.Save();

                
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = [ex.ToString()];
                return BadRequest(_response);
            }
        }

        //Endpoint deletes specific TDTask
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIResponse>> DeleteTDTask(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = ["ID cannot be null"];
                return BadRequest(_response);
            }
            var task = await _unitOfWork.TDTask.Get(u => u.Id == id);
            if(task == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }else
            {
                _unitOfWork.TDTask.Remove(task);
                await _unitOfWork.Save();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
                
        }

        //Endpoint updates Status of TDTask entity
        [HttpPatch("markAsDone/{id:int}")]
        public async Task<ActionResult<APIResponse>> MarkTDTaskAsDone(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            var task = await _unitOfWork.TDTask.Get(u => u.Id == id);
            if(task == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess =false;
                return NotFound(_response);
            }
            if(task.Status == "Done")
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = ["Task already marked as done"];
                return BadRequest(_response);
            }
            task.Status = "Done";
            task.CompletionPercentage = 100;
            _unitOfWork.TDTask.Update(task);
            await  _unitOfWork.Save();
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        
    }
}
