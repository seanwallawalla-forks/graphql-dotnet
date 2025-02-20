using System.Reactive.Linq;

namespace GraphQL.Tests.Subscription
{
    public class SubscriptionWithReflectionTests
    {

        public SubscriptionWithReflectionTests()
        {
            SubscriptionSchemaWithReflection.Initialize(new Chat());
        }


        protected async Task<ExecutionResult> ExecuteSubscribeAsync(ExecutionOptions options)
        {
            var executer = new DocumentExecuter();

            var result = await executer.ExecuteAsync(options);

            result.Data.ShouldBeNull();

            return result;
        }

        [Fact]
        public async Task Subscribe()
        {
            /* Given */
            var addedMessage = new Message
            {
                Content = "test",
                From = new MessageFrom
                {
                    DisplayName = "test",
                    Id = "1"
                },
                SentAt = DateTime.Now
            };

            var chat = SubscriptionSchemaWithReflection.Chat;
            var schema = SubscriptionSchemaWithReflection.Schema;

            /* When */
            var result = await ExecuteSubscribeAsync(new ExecutionOptions
            {
                Query = "subscription MessageAdded { messageAdded { from { id displayName } content sentAt } }",
                Schema = schema
            });

            chat.AddMessage(addedMessage);

            /* Then */
            var stream = result.Streams.Values.FirstOrDefault();
            var message = await stream.FirstOrDefaultAsync();

            message.ShouldNotBeNull();
            message.ShouldBeOfType<ExecutionResult>();
            message.Data.ShouldNotBeNull();
            message.Data.ShouldNotBeAssignableTo<Task>();
        }

        [Fact]
        public async Task SubscribeAsync()
        {
            /* Given */
            var addedMessage = new Message
            {
                Content = "test",
                From = new MessageFrom
                {
                    DisplayName = "test",
                    Id = "1"
                },
                SentAt = DateTime.Now
            };

            var chat = SubscriptionSchemaWithReflection.Chat;
            var schema = SubscriptionSchemaWithReflection.Schema;

            /* When */
            var result = await ExecuteSubscribeAsync(new ExecutionOptions
            {
                Query = "subscription MessageAdded { messageAddedAsync { from { id displayName } content sentAt } }",
                Schema = schema
            });

            chat.AddMessage(addedMessage);

            /* Then */
            var stream = result.Streams.Values.FirstOrDefault();
            var message = await stream.FirstOrDefaultAsync();

            message.ShouldNotBeNull();
            message.ShouldBeOfType<ExecutionResult>();
            message.Data.ShouldNotBeNull();
            message.Data.ShouldNotBeAssignableTo<Task>();
        }

        [Fact]
        public async Task SubscribeWithArgument()
        {
            /* Given */
            var addedMessage = new Message
            {
                Content = "test",
                From = new MessageFrom
                {
                    DisplayName = "test",
                    Id = "1"
                },
                SentAt = DateTime.Now
            };

            var chat = SubscriptionSchemaWithReflection.Chat;
            var schema = SubscriptionSchemaWithReflection.Schema;

            /* When */
            var result = await ExecuteSubscribeAsync(new ExecutionOptions
            {
                Query = "subscription MessageAddedByUser($id:String!) { messageAddedByUser(id: $id) { from { id displayName } content sentAt } }",
                Schema = schema,
                Variables = new Inputs(new Dictionary<string, object>
                {
                    ["id"] = "1"
                })
            });

            chat.AddMessage(addedMessage);

            /* Then */
            var stream = result.Streams.Values.FirstOrDefault();
            var message = await stream.FirstOrDefaultAsync();

            message.ShouldNotBeNull();
            message.ShouldBeOfType<ExecutionResult>();
            message.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task SubscribeWithArgumentAsync()
        {
            /* Given */
            var addedMessage = new Message
            {
                Content = "test",
                From = new MessageFrom
                {
                    DisplayName = "test",
                    Id = "1"
                },
                SentAt = DateTime.Now
            };

            var chat = SubscriptionSchemaWithReflection.Chat;
            var schema = SubscriptionSchemaWithReflection.Schema;

            /* When */
            var result = await ExecuteSubscribeAsync(new ExecutionOptions
            {
                Query = "subscription MessageAddedByUser($id:String!) { messageAddedByUserAsync(id: $id) { from { id displayName } content sentAt } }",
                Schema = schema,
                Variables = new Inputs(new Dictionary<string, object>
                {
                    ["id"] = "1"
                })
            });

            chat.AddMessage(addedMessage);

            /* Then */
            var stream = result.Streams.Values.FirstOrDefault();
            var message = await stream.FirstOrDefaultAsync();

            message.ShouldNotBeNull();
            message.ShouldBeOfType<ExecutionResult>();
            message.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task OnError()
        {
            /* Given */
            var chat = SubscriptionSchemaWithReflection.Chat;
            var schema = SubscriptionSchemaWithReflection.Schema;

            /* When */
            var result = await ExecuteSubscribeAsync(new ExecutionOptions
            {
                Query = "subscription MessageAdded { messageAdded { from { id displayName } content sentAt } }",
                Schema = schema
            });

            chat.AddError(new Exception("test"));

            /* Then */
            var stream = result.Streams.Values.FirstOrDefault();
            var error = await Should.ThrowAsync<ExecutionError>(async () => await stream.FirstOrDefaultAsync());
            error.InnerException.Message.ShouldBe("test");
            error.Path.ShouldBe(new[] { "messageAdded" });
        }
    }
}
