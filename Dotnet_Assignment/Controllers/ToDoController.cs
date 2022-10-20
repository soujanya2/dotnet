using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ToDOList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly string _connectionSting;
        private readonly IConfiguration _configuration;

        public ToDoController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            _connectionSting = _configuration.GetValue<string>("PGConnectionString");
        }

        [HttpGet]
        public IEnumerable<ToDoListModel> Get(bool status = false)
        {
            var todoList = new List<ToDoListModel>();
            try
            {
                using var con = new NpgsqlConnection(_connectionSting);
                System.Console.WriteLine(_connectionSting);
                con.Open();
                var sql = $"SELECT * from todoitem where iscompleted = {status}";
                Console.WriteLine(sql);
                
                using var cmd = new NpgsqlCommand(sql, con);
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    todoList.Add(new ToDoListModel() { 
                        Id = rdr.GetInt32(0), 
                        Name = rdr.GetString(1), 
                        IsCompleted = rdr.GetBoolean(2) 
                    });
                }
                return todoList;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return todoList;
            }
            
        }

        [HttpPost]
        public ToDoListModel Post(ToDoListModel toDoListModel)
        {
            using var con = new NpgsqlConnection(_connectionSting);
            con.Open();
            var sql = $"INSERT INTO public.todoitem(name) VALUES('{toDoListModel.Name}');";
            Console.WriteLine(sql);
            using var cmd = new NpgsqlCommand(sql, con);
            var version = cmd.ExecuteScalar();
            return toDoListModel;
        }

        [HttpPatch]
        public ToDoListModel Patch(ToDoListModel toDoListModel)
        {
            using var con = new NpgsqlConnection(_connectionSting);
            con.Open();
            var sql = $"UPDATE public.todoitem SET name='{toDoListModel.Name}', iscompleted={toDoListModel.IsCompleted} WHERE id = {toDoListModel.Id};";
            Console.WriteLine(sql); 
            using var cmd = new NpgsqlCommand(sql, con);
            var version = cmd.ExecuteScalar();
            return toDoListModel;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            try
            {
                using var con = new NpgsqlConnection(_connectionSting);
                con.Open();
                var sql = $"UPDATE public.todoitem SET iscompleted=true WHERE id = {id};";
                Console.WriteLine(sql);
                using var cmd = new NpgsqlCommand(sql, con);
                var version = cmd.ExecuteScalar();
                return "Success";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

    }
}
