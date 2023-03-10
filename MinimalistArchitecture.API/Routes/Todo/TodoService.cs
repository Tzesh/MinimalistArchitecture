using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MinimalistArchitecture.Common;
using MinimalistArchitecture.Common.Abstract;

namespace MinimalistArchitecture.Routes.Todo;
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
        todoItems.MapGet("/", GetAll);
        todoItems.MapGet("/{id}", Get);
        todoItems.MapPost("/", Create);
        todoItems.MapPut("/{id}", Update);
        todoItems.MapDelete("/{id}", Delete);
    }

    [Authorize]
    public async Task<IResult> GetAll(TodoDb db)
    {
        return TypedResults.Ok(await db.Todos.Select(x => new TodoDTO(x)).ToArrayAsync());
    }

    [Authorize]
    public async Task<IResult> Get(Guid id, TodoDb db)
    {
        return await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(new TodoDTO(todo))
                : TypedResults.NotFound();
    }           

    [Authorize]
    public async Task<IResult> Create([Validate] TodoDTO todoItemDTO, TodoDb db)
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
    public async Task<IResult> Update(Guid id, [Validate] TodoDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsComplete;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    [Authorize]
    public async Task<IResult> Delete(int id, TodoDb db)
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