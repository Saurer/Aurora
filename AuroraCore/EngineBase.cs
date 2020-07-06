using System;
using System.Collections.Generic;
using AuroraCore.Controllers;
using AuroraCore.Events;
using AuroraCore.Storage;
using AuroraCore.Transactions;
using AuroraCore.Types;

namespace AuroraCore {
    public class EngineBase {
        private TransientState state = new TransientState();
        private List<Controller> controllers = new List<Controller>();
        private Dictionary<int, List<Action<IEventData, Transaction>>> reactions = new Dictionary<int, List<Action<IEventData, Transaction>>>();

        public ITransientState State {
            get {
                return state;
            }
        }

        public void AddReaction(int eventID, Action<IEventData, Transaction> reaction) {
            if (!reactions.ContainsKey(eventID)) {
                reactions[eventID] = new List<Action<IEventData, Transaction>>();
            }

            reactions[eventID].Add(reaction);
        }

        public IEnumerable<Action<IEventData, Transaction>> GetReactionsFor(IEventData e) {
            int execCount = 0;

            foreach (var reaction in reactions) {
                if (state.IsEventAncestor(reaction.Key, e.ValueID)) {
                    foreach (var item in reaction.Value) {
                        execCount++;
                        yield return item;
                    }
                }
            }

            if (0 == execCount) {
                throw new Exception("No reactions defined for " + e.ID);
            }

            yield break;
        }

        public void AddController<T>() where T : Controller, new() {
            Controller controller = Controller.Instantiate<T>(State);
            IEnumerable<ReactionBase> reactions = controller.GetReactions();

            foreach (var reaction in reactions) {
                AddReaction(reaction.EventID, (e, tx) => {
                    reaction.Method.Invoke(controller, new object[] {
                        e, tx
                    });
                });
            }
            controllers.Add(controller);
        }

        public void AddType<T>(string name) where T : DataType {
            var types = (TypeManager)state.Types;
            types.Register<T>(name);
        }

        public void ProcessEvent(IEventData e) {
            Transaction tx = new Transaction(state.AttributeModel);
            foreach (var handler in GetReactionsFor(e)) {
                handler(e, tx);
            }
            ProcessTransaction(tx);
        }

        public void ProcessTransaction(Transaction tx) {
            foreach (var item in tx.Events) {
                state.Register(item);
            }

            foreach (var item in tx.Values) {
                if (item is Model model && model.BaseEventID == StaticEvent.Attribute) {
                    state.AttributeModel = new AttrModel(model, model.Parent);
                }
                state.AddValue(item.ID, item);
            }

            foreach (var item in tx.AttrProperties) {
                if (null == state.AttributeModel) {
                    state.AddPendingAttrProperty(item);
                }
                else {
                    state.AttributeModel.RegisterProperty(item);
                    state.FlushAttrProperties(item.ID);
                }
            }

            foreach (var item in tx.AttrPropertyValues) {
                AttrProperty property;

                if (null == state.AttributeModel) {
                    property = state.GetPendingAttrProperty(item.Item1);
                }
                else {
                    property = (AttrProperty)state.AttributeModel.GetProperty(item.Item1);
                }

                property.RegisterValue(item.Item2, item.Item3);
            }

            foreach (var item in tx.IndividualAttributes) {
                if (state.TryGetValue<Individual>(item.Item1, out var individual)) {
                    individual.SetAttribute(item.Item2, item.Item3);
                }
                else {
                    throw new Exception("Transaction error");
                }
            }

            foreach (var item in tx.ModelAttributes) {
                if (state.TryGetValue<Model>(item.Item1, out var model)) {
                    model.AddAttribute(item.Item2);
                }
                else {
                    throw new Exception("Transaction error");
                }
            }

            foreach (var item in tx.AttributeProperties) {
                if (state.TryGetValue<Attr>(item.Item1, out var attr)) {
                    if (item.Item2 == StaticEvent.DataType) {
                        attr.SetDataType(item.Item2, item.Item3, State.Types.Get(item.Item3));
                    }
                    else {
                        attr.SetProperty(item.Item2, item.Item3);
                    }
                }
            }

            foreach (var item in tx.TypeActivations) {
                var types = (TypeManager)state.Types;
                types.Activate(item.Item1, item.Item2);
            }
        }
    }
}