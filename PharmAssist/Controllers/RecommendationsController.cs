using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Core.Services;
using PharmAssist.DTOs;
using PharmAssist.Extensions;
using System.Security.Claims;

namespace PharmAssist.Controllers
{
    [Authorize]
    public class RecommendationsController : APIBaseController
    {
        private readonly IMedicationRecommendationService _recommendationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public RecommendationsController(
            IMedicationRecommendationService recommendationService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _recommendationService = recommendationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("GetMyRecommendations")]
        public async Task<ActionResult> GetMyRecommendations(
            [FromQuery] bool includeConflicted = false,
            [FromQuery] int maxResults = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Check if user profile is complete
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (!_recommendationService.IsUserProfileComplete(user))
                {
                    var incompleteResponse = new ProfileIncompleteResponseDTO
                    {
                        IsProfileComplete = false,
                        Title = "Complete Your Medical Profile",
                        Message = "To provide you with personalized and safe medication recommendations, we need some information about your medical history.",
                        MissingFields = GetMissingFields(user),
                        ActionRequired = "Please complete your medical profile by answering the health questionnaire.",
                        GeneratedAt = DateTime.UtcNow
                    };
                    return Ok(incompleteResponse);
                }

                var recommendations = await _recommendationService.GenerateRecommendationsAsync(userId, includeConflicted, maxResults);
                var (totalSafe, totalConflicted, summary) = await _recommendationService.GetSafetySummaryAsync(userId);

                var recommendationDTOs = recommendations.Select(r => new MedicationRecommendationDTO
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    ProductName = r.Product?.Name ?? "",
                    ProductDescription = r.Product?.Description ?? "",
                    ProductPrice = r.Product?.Price ?? 0,
                    ProductPictureUrl = r.Product?.PictureUrl ?? "",
                    ActiveIngredient = r.Product?.ActiveIngredient ?? "",
                    SafetyScore = r.SafetyScore,
                    EffectivenessScore = r.EffectivenessScore,
                    ValueScore = r.ValueScore,
                    FinalScore = r.FinalScore,
                    HasConflict = r.HasConflict,
                    ConflictDetails = r.ConflictDetails,
                    RecommendationReason = r.RecommendationReason,
                    CreatedAt = r.CreatedAt
                }).ToList();

                var response = new RecommendationResponseDTO
                {
                    Recommendations = recommendationDTOs,
                    Summary = summary,
                    TotalSafeRecommendations = totalSafe,
                    TotalConflictedItems = totalConflicted,
                    GeneratedAt = DateTime.UtcNow,
                    AIPersonalizedMessage = GeneratePersonalizedMessage(user, totalSafe, totalConflicted),
                    ConfidenceLevel = GetConfidenceLevel(recommendationDTOs),
                    KeyInsights = GenerateKeyInsights(recommendationDTOs, user),
                    NextStepsAdvice = GenerateNextStepsAdvice(totalSafe, totalConflicted, user)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating recommendations: {ex.Message}");
            }
        }

        [HttpGet("GetSafetySummary")]
        public async Task<ActionResult> GetSafetySummary()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Check if user profile is complete
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (!_recommendationService.IsUserProfileComplete(user))
                {
                    var incompleteResponse = new ProfileIncompleteResponseDTO
                    {
                        IsProfileComplete = false,
                        Title = "Complete Your Medical Profile First",
                        Message = "We cannot provide a safety summary without your medical information. Please complete your profile to receive personalized safety recommendations.",
                        MissingFields = GetMissingFields(user),
                        ActionRequired = "Complete your medical profile to get safety recommendations.",
                        GeneratedAt = DateTime.UtcNow
                    };
                    return Ok(incompleteResponse);
                }

                var (totalSafe, totalConflicted, summary) = await _recommendationService.GetSafetySummaryAsync(userId);
                
                // Get top recommendation for display
                var recommendations = await _recommendationService.GenerateRecommendationsAsync(userId, false, 1);
                var topRecommendation = recommendations.FirstOrDefault();

                MedicationRecommendationDTO? topRecommendationDTO = null;
                if (topRecommendation != null)
                {
                    topRecommendationDTO = new MedicationRecommendationDTO
                    {
                        Id = topRecommendation.Id,
                        ProductId = topRecommendation.ProductId,
                        ProductName = topRecommendation.Product?.Name ?? "",
                        ProductDescription = topRecommendation.Product?.Description ?? "",
                        ProductPrice = topRecommendation.Product?.Price ?? 0,
                        ProductPictureUrl = topRecommendation.Product?.PictureUrl ?? "",
                        ActiveIngredient = topRecommendation.Product?.ActiveIngredient ?? "",
                        SafetyScore = topRecommendation.SafetyScore,
                        EffectivenessScore = topRecommendation.EffectivenessScore,
                        ValueScore = topRecommendation.ValueScore,
                        FinalScore = topRecommendation.FinalScore,
                        HasConflict = topRecommendation.HasConflict,
                        ConflictDetails = topRecommendation.ConflictDetails,
                        RecommendationReason = topRecommendation.RecommendationReason,
                        CreatedAt = topRecommendation.CreatedAt
                    };
                }

                var response = new SafetySummaryDTO
                {
                    TotalSafeRecommendations = totalSafe,
                    TotalConflictedItems = totalConflicted,
                    Summary = summary,
                    TopRecommendation = topRecommendationDTO,
                    GeneratedAt = DateTime.UtcNow,
                    AIPersonalizedMessage = GeneratePersonalizedSafetyMessage(user, totalSafe, totalConflicted),
                    OverallRiskAssessment = GenerateRiskAssessment(totalSafe, totalConflicted, user),
                    SafetyHighlights = GenerateSafetyHighlights(totalSafe, totalConflicted, user),
                    RecommendedAction = GenerateRecommendedAction(totalSafe, totalConflicted, user)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting safety summary: {ex.Message}");
            }
        }

        [HttpGet("CheckProductSafety/{productId}")]
        public async Task<ActionResult<MedicationRecommendationDTO>> CheckProductSafety(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found");
                }

                var recommendation = await _recommendationService.AnalyzeProductSafetyAsync(product, userId);
                var recommendationDTO = new MedicationRecommendationDTO
                {
                    Id = recommendation.Id,
                    ProductId = recommendation.ProductId,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    ProductPrice = product.Price,
                    ProductPictureUrl = product.PictureUrl,
                    ActiveIngredient = product.ActiveIngredient,
                    SafetyScore = recommendation.SafetyScore,
                    EffectivenessScore = recommendation.EffectivenessScore,
                    ValueScore = recommendation.ValueScore,
                    FinalScore = recommendation.FinalScore,
                    HasConflict = recommendation.HasConflict,
                    ConflictDetails = recommendation.ConflictDetails,
                    RecommendationReason = recommendation.RecommendationReason,
                    CreatedAt = recommendation.CreatedAt
                };

                return Ok(recommendationDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking product safety: {ex.Message}");
            }
        }

        [HttpGet("GetConflictingMedications")]
        public async Task<ActionResult> GetConflictingMedications([FromQuery] int maxResults = 50)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Check if user profile is complete
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (!_recommendationService.IsUserProfileComplete(user))
                {
                    var incompleteResponse = new ProfileIncompleteResponseDTO
                    {
                        IsProfileComplete = false,
                        Title = "Complete Your Medical Profile First",
                        Message = "We need your medical information to identify medications that may conflict with your chronic conditions.",
                        MissingFields = GetMissingFields(user),
                        ActionRequired = "Complete your medical profile to see potential medication conflicts.",
                        GeneratedAt = DateTime.UtcNow
                    };
                    return Ok(incompleteResponse);
                }

                // Get user's medical conditions for personalized messages
                var medicalProfile = await _recommendationService.GetUserMedicalProfileAsync(userId);
                var userConditions = medicalProfile.ContainsKey("ParsedConditions") ? 
                    medicalProfile["ParsedConditions"] as List<string> : new List<string>();

                // Filter to only include chronic conditions
                var chronicConditions = userConditions.Where(c => 
                    c == "diabetes" || 
                    c == "hypertension" || 
                    c == "heart_disease" || 
                    c == "kidney_disease" || 
                    c == "liver_disease" || 
                    c == "asthma" || 
                    c == "copd" || 
                    c == "thyroid_disease" || 
                    c == "arthritis" || 
                    c == "inflammatory_arthritis" || 
                    c == "osteoporosis" || 
                    c == "gout" || 
                    c == "seizures" || 
                    c == "depression" || 
                    c == "myasthenia_gravis" || 
                    c == "g6pd_deficiency" || 
                    c == "autoimmune_diseases").ToList();

                if (!chronicConditions.Any())
                {
                    var emptyResponse = new ConflictingMedicationsResponseDTO
                    {
                        ConflictingMedications = new List<MedicationRecommendationDTO>(),
                        TotalConflictingItems = 0,
                        Summary = "No chronic conditions detected in your medical profile.",
                        GeneratedAt = DateTime.UtcNow,
                        AIPersonalizedMessage = $"{user.DisplayName}, I haven't identified any chronic conditions in your profile that would have medication conflicts.",
                        ConflictWarnings = new List<string>(),
                        SafetyAdvice = new List<string> { 
                            "Always consult with your healthcare provider before starting any new medication.",
                            "Keep your medical profile updated with any chronic conditions for accurate conflict detection."
                        }
                    };
                    return Ok(emptyResponse);
                }

                // Get conflicting medications
                var conflictingMedications = await _recommendationService.GetConflictingMedicationsAsync(userId, maxResults);
                
                if (!conflictingMedications.Any())
                {
                    var emptyResponse = new ConflictingMedicationsResponseDTO
                    {
                        ConflictingMedications = new List<MedicationRecommendationDTO>(),
                        TotalConflictingItems = 0,
                        Summary = "No medications found that conflict with your chronic conditions.",
                        GeneratedAt = DateTime.UtcNow,
                        AIPersonalizedMessage = $"Good news, {user.DisplayName}! I haven't identified any medications that would conflict with your chronic conditions.",
                        ConflictWarnings = new List<string>(),
                        SafetyAdvice = new List<string> { 
                            "Always consult with your healthcare provider before starting any new medication.",
                            "Keep your medical profile updated for the most accurate conflict detection."
                        }
                    };
                    return Ok(emptyResponse);
                }

                // Convert to DTOs
                var conflictingMedicationDTOs = conflictingMedications.Select(r => new MedicationRecommendationDTO
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    ProductName = r.Product?.Name ?? "",
                    ProductDescription = r.Product?.Description ?? "",
                    ProductPrice = r.Product?.Price ?? 0,
                    ProductPictureUrl = r.Product?.PictureUrl ?? "",
                    ActiveIngredient = r.Product?.ActiveIngredient ?? "",
                    SafetyScore = r.SafetyScore,
                    EffectivenessScore = r.EffectivenessScore,
                    ValueScore = r.ValueScore,
                    FinalScore = r.FinalScore,
                    HasConflict = r.HasConflict,
                    ConflictDetails = r.ConflictDetails,
                    RecommendationReason = r.RecommendationReason,
                    CreatedAt = r.CreatedAt
                }).ToList();

                // Generate conflict warnings based on chronic conditions
                var conflictWarnings = GenerateChronicConditionWarnings(chronicConditions);
                
                // Generate summary
                var summary = $"Found {conflictingMedications.Count} medications that may conflict with your chronic health conditions.";
                
                // Create response
                var response = new ConflictingMedicationsResponseDTO
                {
                    ConflictingMedications = conflictingMedicationDTOs,
                    TotalConflictingItems = conflictingMedications.Count,
                    Summary = summary,
                    GeneratedAt = DateTime.UtcNow,
                    AIPersonalizedMessage = GenerateChronicConditionConflictMessage(user, conflictingMedications.Count, chronicConditions),
                    ConflictWarnings = conflictWarnings,
                    SafetyAdvice = GenerateChronicConditionSafetyAdvice(chronicConditions)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting conflicting medications: {ex.Message}");
            }
        }

        [HttpGet("GetMyMedicalProfile")]
        public async Task<ActionResult<MedicalProfileDTO>> GetMyMedicalProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var profile = await _recommendationService.GetUserMedicalProfileAsync(userId);

                var profileDTO = new MedicalProfileDTO
                {
                    UserId = profile.GetValueOrDefault("UserId")?.ToString() ?? "",
                    DisplayName = profile.GetValueOrDefault("DisplayName")?.ToString() ?? "",
                    PromptReason = profile.GetValueOrDefault("PromptReason")?.ToString() ?? "",
                    HasChronicConditions = profile.GetValueOrDefault("HasChronicConditions")?.ToString() ?? "",
                    TakesMedicationsOrTreatments = profile.GetValueOrDefault("TakesMedicationsOrTreatments")?.ToString() ?? "",
                    CurrentSymptoms = profile.GetValueOrDefault("CurrentSymptoms")?.ToString() ?? "",
                    ParsedConditions = (profile.GetValueOrDefault("ParsedConditions") as List<string>) ?? new List<string>()
                };

                return Ok(profileDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting medical profile: {ex.Message}");
            }
        }

        [HttpGet("CheckProfileCompletion")]
        public async Task<ActionResult> CheckProfileCompletion()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var isComplete = _recommendationService.IsUserProfileComplete(user);

                if (isComplete)
                {
                    return Ok(new 
                    { 
                        IsProfileComplete = true,
                        Message = "Your medical profile is complete and ready for recommendations.",
                        Title = "Profile Complete"
                    });
                }
                else
                {
                    var incompleteResponse = new ProfileIncompleteResponseDTO
                    {
                        IsProfileComplete = false,
                        Title = "Medical Profile Incomplete",
                        Message = "Complete your medical profile to receive personalized medication recommendations and safety alerts.",
                        MissingFields = GetMissingFields(user),
                        ActionRequired = "Please fill out the missing medical information in your profile.",
                        GeneratedAt = DateTime.UtcNow
                    };
                    return Ok(incompleteResponse);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking profile completion: {ex.Message}");
            }
        }

        [HttpPost("RefreshRecommendations")]
        public async Task<ActionResult> RefreshRecommendations(
            [FromQuery] bool includeConflicted = false,
            [FromQuery] int maxResults = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Check if user profile is complete
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (!_recommendationService.IsUserProfileComplete(user))
                {
                    var incompleteResponse = new ProfileIncompleteResponseDTO
                    {
                        IsProfileComplete = false,
                        Title = "Cannot Generate Recommendations",
                        Message = "Your medical profile must be completed before we can generate fresh recommendations for you.",
                        MissingFields = GetMissingFields(user),
                        ActionRequired = "Please complete your medical profile first, then try refreshing recommendations.",
                        GeneratedAt = DateTime.UtcNow
                    };
                    return Ok(incompleteResponse);
                }

                // Force regeneration of recommendations
                var recommendations = await _recommendationService.GenerateRecommendationsAsync(userId, includeConflicted, maxResults);
                var (totalSafe, totalConflicted, summary) = await _recommendationService.GetSafetySummaryAsync(userId);

                var recommendationDTOs = recommendations.Select(r => new MedicationRecommendationDTO
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    ProductName = r.Product?.Name ?? "",
                    ProductDescription = r.Product?.Description ?? "",
                    ProductPrice = r.Product?.Price ?? 0,
                    ProductPictureUrl = r.Product?.PictureUrl ?? "",
                    ActiveIngredient = r.Product?.ActiveIngredient ?? "",
                    SafetyScore = r.SafetyScore,
                    EffectivenessScore = r.EffectivenessScore,
                    ValueScore = r.ValueScore,
                    FinalScore = r.FinalScore,
                    HasConflict = r.HasConflict,
                    ConflictDetails = r.ConflictDetails,
                    RecommendationReason = r.RecommendationReason,
                    CreatedAt = r.CreatedAt
                }).ToList();

                var response = new RecommendationResponseDTO
                {
                    Recommendations = recommendationDTOs,
                    Summary = summary,
                    TotalSafeRecommendations = totalSafe,
                    TotalConflictedItems = totalConflicted,
                    GeneratedAt = DateTime.UtcNow,
                    AIPersonalizedMessage = GeneratePersonalizedMessage(user, totalSafe, totalConflicted),
                    ConfidenceLevel = GetConfidenceLevel(recommendationDTOs),
                    KeyInsights = GenerateKeyInsights(recommendationDTOs, user),
                    NextStepsAdvice = GenerateNextStepsAdvice(totalSafe, totalConflicted, user)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error refreshing recommendations: {ex.Message}");
            }
        }

        private List<string> GetMissingFields(AppUser user)
        {
            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(user.PromptReason))
            {
                missingFields.Add("Reason for visit/consultation");
            }

            if (string.IsNullOrWhiteSpace(user.HasChronicConditions))
            {
                missingFields.Add("Chronic conditions and medical history");
            }

            if (string.IsNullOrWhiteSpace(user.TakesMedicationsOrTreatments))
            {
                missingFields.Add("Current medications and treatments");
            }

            if (string.IsNullOrWhiteSpace(user.CurrentSymptoms))
            {
                missingFields.Add("Current symptoms and concerns");
            }

            if (missingFields.Count == 0)
            {
                missingFields.Add("Medical profile appears complete, but may need additional information");
            }

            return missingFields;
        }

        private string GeneratePersonalizedMessage(AppUser user, int totalSafe, int totalConflicted)
        {
            var displayName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : "there";
            
            if (totalSafe == 0)
            {
                return $"Hello {displayName}, I've completed a thorough analysis of your medical profile and available medications. While I found some options that require careful consideration, I want to prioritize your safety above all else.";
            }
            else if (totalSafe == 1)
            {
                return $"Hi {displayName}! I'm pleased to share that I've found a highly suitable medication option for you. Based on your medical history and current needs, I've identified a safe and effective choice that aligns well with your health profile.";
            }
            else
            {
                return $"Hello {displayName}! Great news - I've analyzed your medical profile and found {totalSafe} excellent medication options that are both safe and effective for your specific needs. I've ranked them based on safety, effectiveness, and value to help you make the best choice.";
            }
        }

        private string GetConfidenceLevel(List<MedicationRecommendationDTO> recommendations)
        {
            if (!recommendations.Any()) return "Low";
            
            var avgScore = recommendations.Average(r => r.FinalScore);
            var hasHighScores = recommendations.Any(r => r.FinalScore >= 4.0);
            var hasConflicts = recommendations.Any(r => r.HasConflict);
            
            if (avgScore >= 4.0 && hasHighScores && !hasConflicts)
                return "Very High";
            else if (avgScore >= 3.5 && !hasConflicts)
                return "High";
            else if (avgScore >= 3.0)
                return "Moderate";
            else
                return "Low";
        }

        private List<string> GenerateKeyInsights(List<MedicationRecommendationDTO> recommendations, AppUser user)
        {
            if (!recommendations.Any())
            {
                return new List<string> { "Your medical profile requires careful consideration for medication selection" };
            }

            // Convert DTOs to domain models for the service call
            var domainRecommendations = recommendations.Select(dto => new MedicationRecommendation
            {
                Id = dto.Id,
                ProductId = dto.ProductId,
                Product = new Product 
                { 
                    Name = dto.ProductName, 
                    Price = dto.ProductPrice,
                    ActiveIngredient = dto.ActiveIngredient
                },
                SafetyScore = dto.SafetyScore,
                EffectivenessScore = dto.EffectivenessScore,
                ValueScore = dto.ValueScore,
                FinalScore = dto.FinalScore,
                HasConflict = dto.HasConflict,
                ConflictDetails = dto.ConflictDetails
            }).ToList();

            // Get user conditions
            var userProfile = _recommendationService.GetUserMedicalProfileAsync(user.Id).Result;
            var userConditions = (userProfile.GetValueOrDefault("ParsedConditions") as List<string>) ?? new List<string>();

            // Use the enhanced AI insights
            return _recommendationService.GenerateIntelligentInsights(domainRecommendations, userConditions);
        }

        private string GenerateNextStepsAdvice(int totalSafe, int totalConflicted, AppUser user)
        {
            if (totalSafe == 0)
            {
                return "I recommend consulting with your healthcare provider to discuss specialized treatment options that can be safely monitored for your specific medical conditions.";
            }
            else if (totalSafe == 1)
            {
                return "Consider discussing this recommendation with your pharmacist or healthcare provider to ensure it fits perfectly with your current treatment plan, then you can proceed with confidence.";
            }
            else
            {
                return "Review the detailed analysis of each recommendation, discuss your top choices with your healthcare provider, and feel free to ask your pharmacist any questions about usage or interactions.";
            }
        }

        private string GeneratePersonalizedSafetyMessage(AppUser user, int totalSafe, int totalConflicted)
        {
            var displayName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : "there";
            
            if (totalSafe == 0)
            {
                return $"Hi {displayName}, I've prioritized your safety in this analysis. While I found some medications that could be helpful, they all require professional medical supervision given your specific health profile.";
            }
            else
            {
                return $"Hello {displayName}! I'm happy to report that I've found {totalSafe} medication(s) that are safe for your specific medical conditions. Your safety is my top priority, and these options have passed all safety checks.";
            }
        }

        private string GenerateRiskAssessment(int totalSafe, int totalConflicted, AppUser user)
        {
            if (totalSafe == 0 && totalConflicted > 0)
            {
                return "Moderate to High Risk - All available options require medical supervision";
            }
            else if (totalSafe > 0 && totalConflicted == 0)
            {
                return "Low Risk - All recommendations are safe for your medical profile";
            }
            else if (totalSafe > totalConflicted)
            {
                return "Low to Moderate Risk - Multiple safe options available with some requiring supervision";
            }
            else
            {
                return "Moderate Risk - Equal number of safe and supervised options available";
            }
        }

        private List<string> GenerateSafetyHighlights(int totalSafe, int totalConflicted, AppUser user)
        {
            var highlights = new List<string>();
            
            if (totalSafe > 0)
            {
                highlights.Add($"✅ {totalSafe} medication(s) cleared all safety checks for your medical profile");
            }
            
            if (totalConflicted > 0)
            {
                highlights.Add($"⚠️ {totalConflicted} medication(s) require medical supervision due to potential interactions");
            }
            
            if (!string.IsNullOrEmpty(user.HasChronicConditions))
            {
                highlights.Add("🔍 Special attention given to your chronic health conditions");
            }
            
            if (!string.IsNullOrEmpty(user.TakesMedicationsOrTreatments))
            {
                highlights.Add("💊 Current medications considered in safety analysis");
            }
            
            highlights.Add("🧬 Personalized analysis based on your unique medical profile");
            
            return highlights;
        }

        private string GenerateRecommendedAction(int totalSafe, int totalConflicted, AppUser user)
        {
            if (totalSafe == 0)
            {
                return "Schedule a consultation with your healthcare provider to discuss supervised treatment options that can be safely monitored for your specific needs.";
            }
            else if (totalSafe == 1)
            {
                return "Proceed with confidence by discussing the recommended option with your pharmacist, then follow the recommended dosage and usage instructions.";
            }
            else
            {
                return "Choose from the safe options based on your preferences and budget, consult with your pharmacist for any questions, and always follow proper usage guidelines.";
            }
        }

        private List<string> GenerateChronicConditionWarnings(List<string> chronicConditions)
        {
            var warnings = new List<string>();
            
            if (chronicConditions.Contains("diabetes"))
            {
                warnings.Add("⚠️ Diabetes: Some medications can affect blood sugar control. Monitor your levels closely when starting new medications.");
            }
            
            if (chronicConditions.Contains("hypertension"))
            {
                warnings.Add("⚠️ Hypertension: NSAIDs, decongestants, and some supplements can raise blood pressure.");
            }
            
            if (chronicConditions.Contains("kidney_disease"))
            {
                warnings.Add("⚠️ Kidney disease: NSAIDs, certain antibiotics, and contrast agents can worsen kidney function.");
            }
            
            if (chronicConditions.Contains("liver_disease"))
            {
                warnings.Add("⚠️ Liver disease: Acetaminophen, certain statins, and some antifungals can affect liver function.");
            }
            
            if (chronicConditions.Contains("heart_disease"))
            {
                warnings.Add("⚠️ Heart disease: Some decongestants, NSAIDs, and certain antibiotics may affect heart function.");
            }

            if (chronicConditions.Contains("asthma"))
            {
                warnings.Add("⚠️ Asthma: NSAIDs and beta-blockers can trigger asthma symptoms in some people.");
            }

            if (chronicConditions.Contains("copd"))
            {
                warnings.Add("⚠️ COPD: Beta-blockers, sedatives, and certain opioids can worsen respiratory function.");
            }

            if (chronicConditions.Contains("thyroid_disease"))
            {
                warnings.Add("⚠️ Thyroid disease: Some medications can interfere with thyroid hormone absorption or function.");
            }

            if (chronicConditions.Contains("arthritis") || chronicConditions.Contains("inflammatory_arthritis"))
            {
                warnings.Add("⚠️ Arthritis: Some medications may interact with your arthritis treatments or worsen joint inflammation.");
            }

            // Add a general warning if no specific conditions matched
            if (warnings.Count == 0 && chronicConditions.Any())
            {
                warnings.Add("⚠️ Always check with your healthcare provider before taking new medications, especially with your chronic conditions.");
            }

            return warnings;
        }

        private List<string> GenerateChronicConditionSafetyAdvice(List<string> chronicConditions)
        {
            var advice = new List<string>
            {
                "Always inform healthcare providers about all your chronic conditions and medications.",
                "Keep an updated list of your allergies and medical conditions to share with healthcare providers.",
                "When prescribed a new medication, ask specifically about interactions with your chronic conditions."
            };

            // Add condition-specific advice
            if (chronicConditions.Contains("diabetes") || chronicConditions.Contains("hypertension"))
            {
                advice.Add("Monitor your vital signs regularly when starting new medications that may affect your chronic conditions.");
            }

            if (chronicConditions.Contains("kidney_disease") || chronicConditions.Contains("liver_disease"))
            {
                advice.Add("Ask your doctor about appropriate dosage adjustments for your condition when taking new medications.");
            }

            if (chronicConditions.Contains("heart_disease"))
            {
                advice.Add("Some medications can affect heart rhythm or function. Discuss any new medications with your cardiologist.");
            }

            return advice;
        }

        private string GenerateChronicConditionConflictMessage(AppUser user, int conflictCount, List<string> chronicConditions)
        {
            string conditionsList = string.Join(", ", chronicConditions.Select(c => c.Replace("_", " ")));
            
            if (conflictCount == 0)
            {
                return $"Good news, {user.DisplayName}! I haven't found any medications that would conflict with your chronic conditions.";
            }
            else if (conflictCount <= 5)
            {
                return $"{user.DisplayName}, I've identified a few medications ({conflictCount}) that could potentially conflict with your {conditionsList}. Review them carefully with your healthcare provider.";
            }
            else if (conflictCount <= 20)
            {
                return $"{user.DisplayName}, I've found several medications ({conflictCount}) that may conflict with your {conditionsList}. This information can help you make safer medication choices.";
            }
            else
            {
                return $"{user.DisplayName}, I've identified a significant number of medications ({conflictCount}) that could conflict with your chronic conditions. This knowledge is important for your medication safety.";
            }
        }
    }
} 