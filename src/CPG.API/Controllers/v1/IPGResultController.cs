using CPG.Domain.SharedKernel.ApplicationSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    [Route("IPGResult")]
    public class IPGResultController : ApiBaseController
    {
        private readonly IApplicationSettingsRepository _applicationSettingsRepository;

        public IPGResultController(IApplicationSettingsRepository applicationSettingsRepository)
        {
            _applicationSettingsRepository = applicationSettingsRepository;
        }
        [HttpPost("p/b/{id}")]  
        public async Task<IActionResult> GetData(string id)
        {
          var appConfig=await  _applicationSettingsRepository.GetAllApplicationSettings();

          return  Redirect($"{appConfig.IPG_Callback_URL}?trackId={id}");
        }
    }
}
