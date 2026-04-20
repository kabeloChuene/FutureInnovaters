using System;
using System.Collections.Generic;

namespace SmartSeatAllocation.Core.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponseDto()
        {
            Errors = new List<string>();
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponseDto<T> Success(T data, string message = "Operation successful")
        {
            return new ApiResponseDto<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDto<T> Failure(string message, params string[] errors)
        {
            return new ApiResponseDto<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = new List<string>(errors)
            };
        }
    }

    public class ApiResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponseDto()
        {
            Errors = new List<string>();
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponseDto Success(object data = null, string message = "Operation successful")
        {
            return new ApiResponseDto
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDto Failure(string message, params string[] errors)
        {
            return new ApiResponseDto
            {
                IsSuccess = false,
                Message = message,
                Errors = new List<string>(errors)
            };
        }
    }

    public class PaginatedResponseDto<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public PaginatedResponseDto()
        {
            Items = new List<T>();
        }

        public PaginatedResponseDto(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasNextPage = pageNumber < TotalPages;
            HasPreviousPage = pageNumber > 1;
        }
    }
}
