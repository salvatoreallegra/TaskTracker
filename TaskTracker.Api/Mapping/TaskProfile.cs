// -------------------------------------------------------
// TaskProfile.cs
// PURPOSE: AutoMapper configuration for TaskItem <-> DTOs.
// Profiles tell AutoMapper how to map between types.
// -------------------------------------------------------
using AutoMapper;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Mapping;

public sealed class TaskProfile : Profile
{
    public TaskProfile()
    {
        // Entity -> Read DTO
        CreateMap<TaskItem, TaskReadDto>();

        // Create DTO -> Entity
        CreateMap<TaskCreateDto, TaskItem>()
            .ForMember(dest => dest.IsDone, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Update DTO -> Entity (ApplyUpdate logic becomes part of AutoMapper)
        CreateMap<TaskUpdateDto, TaskItem>()
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore()); // never update CreatedUtc
    }
}
