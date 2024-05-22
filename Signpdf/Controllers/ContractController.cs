using Microsoft.AspNetCore.Mvc;
using Signpdf.Models;
using Signpdf.Models.DTO_s;
using Signpdf.Services;
using Signpdf.Services.Implements;

namespace Signpdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContractController : Controller
    {
        private readonly SignDbContext _context;
        private readonly ISignService _signService;
        private readonly IContractService _contractService;

        public ContractController(SignDbContext context, ISignService signService, IContractService contractService)
        {
            _context = context;
            _signService = signService;
            _contractService = contractService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file data");
            }

            try
            {
                // Create the folder if it doesn't exist
                var folderPath = Path.Combine("wwwroot", "contract");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save the PDF file to the folder
                var filePath = Path.Combine(folderPath, Guid.NewGuid().ToString() + ".pdf");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Create a new Contract entity
                var newContract = new Contract
                {
                    Title = Path.GetFileNameWithoutExtension(file.FileName),
                    Path = filePath, // Store the file path in the database
                    CreatedAt = DateTime.Now
                };


                // Add the new contract to the database
                _context.Contracts.Add(newContract);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetContracts()
        {
            var contracts = _context.Contracts.ToList();
            return Ok(contracts);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetContractFile(int id, bool isSigned = false)
        {
            var contract = _context.Contracts.Find(id);

            if (contract == null)
            {
                return NotFound(); // Return 404 if the contract is not found
            }

            var filePath = isSigned ? $"wwwroot/signed/{id}.pdf" : contract.Path;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // Return 404 if the file is not found
            }

            // Return the file as a FileStreamResult
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileResult = new FileStreamResult(fileStream, "application/pdf"); // Adjust content type based on your file type
            fileResult.FileDownloadName = $"{contract.Title}.pdf"; // Set the file name

            return fileResult;
        }


        [HttpPost]
        [Route("sign/{contractId}")]
        public IActionResult AddSignatures(int contractId, [FromBody] List<SignatureDTO> signatureDTOs)
        {
            if (signatureDTOs.Count == 0)
                return BadRequest("Signature required");

            var contract = _context.Contracts.Find(contractId);

            if (contract == null)
                return NotFound();

            try
            {
                var signatures = signatureDTOs.Select(dto => new Signature
                {
                    X = dto.X,
                    Y = dto.Y,
                    Width = dto.Width,
                    Height = dto.Height,
                    Name = dto.Name,
                    Reason = dto.Reason,
                    Page = dto.Page,
                    Contractid = contractId
                }).ToList();

                // Add signatures to the contract
                _signService.SignMany($"wwwroot/signed/", signatures, contract);

                var fileStream = new FileStream($"wwwroot/signed/{contract.Id}.pdf", FileMode.Open, FileAccess.Read);

                contract.IsSigned = true;
                _context.SaveChanges();

                return File(fileStream, "application/pdf", $"{contract.Title}.pdf");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("getSignature/{contractId}")]
        public async Task<ActionResult<IEnumerable<Signature>>> GetSignaturesByContractId(int contractId)
        {
            var signatures = await _contractService.GetByContractIdAsync(contractId);

            if (signatures == null || !signatures.Any())
            {
                return NotFound();
            }

            return Ok(signatures);
        }

        [HttpGet]
        [Route("getDetails/{id}")]
        public IActionResult GetContractDetails(int id)
        {
            var contract = _context.Contracts.Find(id);

            if (contract == null)
            {
                return NotFound(); // Return 404 if the contract is not found
            }

            return Ok(contract);
        }

        [HttpPost]
        [Route("save/{contractId}")]
        public async Task<ActionResult<IEnumerable<Signature>>> SaveSignaturesByContractId(int contractId, [FromBody] List<SignatureDTO> signatureDTOs)
        {
            if (signatureDTOs == null || !signatureDTOs.Any())
            {
                return BadRequest("No signatures provided");
            }

            var deleteResult = await _contractService.DeleteAllByContractIdAsync(contractId);

            if (!deleteResult)
            {
                return BadRequest("Failed to delete existing signatures");
            }

            var savedSignatures = new List<Signature>();

            foreach (var dto in signatureDTOs)
            {
                var signature = new Signature
                {
                    X = dto.X,
                    Y = dto.Y,
                    Width = dto.Width,
                    Height = dto.Height,
                    Name = dto.Name,
                    Reason = dto.Reason,
                    Page = dto.Page,
                    Contractid = contractId
                };

                var savedSignature = await _contractService.CreateAsync(signature);
                savedSignatures.Add(savedSignature);
            }

            return Ok(savedSignatures);
        }

    }
}

