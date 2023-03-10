using MinimalistArchitecture.Common.Abstract;

namespace MinimalistArchitecture.Routes.Todo
{
	public class TodoDTO : DTO
	{
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public TodoDTO() { }
        public TodoDTO(Todo todoItem) =>
        (Id, Name, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.IsComplete);
    }
}

