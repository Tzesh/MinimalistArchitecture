using Microsoft.EntityFrameworkCore;
using MinimalistArchitecture.Abstract;

namespace MinimalistArchitecture.Todo;
public class TodoRoutes : Routes
{
    // override the RegisterRoutes method
    new public static void RegisterRoutes(WebApplication app)
    {
        // create a route group
        RouteGroupBuilder todoItems = app.MapGroup("/todo");

        todoItems.MapGet("/", GetAllTodos);
        todoItems.MapGet("/{id}", GetTodo);
        todoItems.MapPost("/", CreateTodo);
        todoItems.MapPut("/{id}", UpdateTodo);
        todoItems.MapDelete("/{id}", DeleteTodo);
    }

    public static async Task<IResult> GetAllTodos(TodoDb db)
    {
        return TypedResults.Ok(await db.Todos.Select(x => new TodoDTO(x)).ToArrayAsync());
    }

    public static async Task<IResult> GetTodo(int id, TodoDb db)
    {
        return await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(new TodoDTO(todo))
                : TypedResults.NotFound();
    }

    public static async Task<IResult> CreateTodo(TodoDTO todoItemDTO, TodoDb db)
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

    public static async Task<IResult> UpdateTodo(int id, TodoDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsComplete;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<IResult> DeleteTodo(int id, TodoDb db)
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