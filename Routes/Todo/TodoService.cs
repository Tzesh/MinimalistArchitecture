using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MinimalistArchitecture.Abstract;

namespace MinimalistArchitecture.Todo;
public class TodoService : Service
{
    // constructor
    public TodoService(WebApplication app) : base(app)
    {
        // create a route group
        RouteGroupBuilder todoItems = app.MapGroup("/todo");

        // add the validation filter to the group
        todoItems.AddEndpointFilterFactory(ValidationFactory.ValidationFilterFactory);

        // add the routes to the group
        todoItems.MapGet("/", GetAllTodos);
        todoItems.MapGet("/{id}", GetTodo);
        todoItems.MapPost("/", CreateTodo);
        todoItems.MapPut("/{id}", UpdateTodo);
        todoItems.MapDelete("/{id}", DeleteTodo);
    }

    [Authorize]
    public async Task<IResult> GetAllTodos(TodoDb db)
    {
        return TypedResults.Ok(await db.Todos.Select(x => new TodoDTO(x)).ToArrayAsync());
    }

    [Authorize]
    public async Task<IResult> GetTodo(int id, TodoDb db)
    {
        return await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(new TodoDTO(todo))
                : TypedResults.NotFound();
    }           

    [Authorize]
    public async Task<IResult> CreateTodo([Validate] TodoDTO todoItemDTO, TodoDb db)
    {
        var todoItem = new Todo
        {
            IsComplete = todoItemDTO.IsComplete,
            Name = todoItemDTO.Name
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
    }

    [Authorize]
    public async Task<IResult> UpdateTodo(int id, [Validate] TodoDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsComplete;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    [Authorize]
    public async Task<IResult> DeleteTodo(int id, TodoDb db)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.Ok(todo);
        }

        return TypedResults.NotFound();
    }
}