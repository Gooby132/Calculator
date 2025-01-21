import logo from "./logo.svg";
import {
  Box,
  Button,
  Combobox,
  Group,
  Input,
  InputBase,
  Stack,
  TextInput,
  Title,
  useCombobox,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import "./App.css";
import { useEffect, useState } from "react";
import axios from "axios";
import { OperationView } from "./OperationView";

const App = () => {
  const [availableOperations, setAvailableOperations] = useState([]);
  const [user, setUser] = useState({
    operations: [],
  });
  const [history, setHistory] = useState(undefined);
  const [lastOperation, setLastOperation] = useState(undefined);

  useEffect(() => {
    const login = async () => {
      try {
        const data = await axios.post(
          "https://localhost:7145/api/Users/login-user",
          {
            internetAddress: "test", // not required
          }
        );

        setUser(data.data.user);
      } catch (error) {
        console.log(error);
      }
    };

    const fetchAvailableOperations = async () => {
      try {
        const data = await axios.get(
          "https://localhost:7145/api/Operations/available-operations"
        );
        setAvailableOperations(data.data.computations);
      } catch (error) {
        console.log(error);
      }
    };

    login();
    fetchAvailableOperations();
  }, []);

  const form = useForm({
    initialValues: {
      value1: "",
      value2: "",
      custom: "",
      operation: 0,
    },
  });

  const availableOperationsOptions = availableOperations.map((operation) => (
    <Combobox.Option key={operation.value} value={operation.value}>
      {operation.name}
    </Combobox.Option>
  ));

  const combobox = useCombobox();

  return (
    <Stack>
      <Stack
        component="form"
        onSubmit={form.onSubmit((val) => {
          axios
            .post("https://localhost:7145/api/Operations/do-operation", {
              userId: user.id,
              operation: val,
            })
            .then((data) => setLastOperation(data.data.operation));
        })}
      >
        <TextInput
          key={form.key("value1")}
          label="Value 1"
          required
          {...form.getInputProps("value1")}
        />

        <Combobox
          store={combobox}
          onOptionSubmit={(option) => {
            form.setFieldValue("operation", option);
            combobox.closeDropdown();
          }}
        >
          <Combobox.Target>
            <InputBase
              component="button"
              type="button"
              pointer
              label="operation"
              rightSection={<Combobox.Chevron />}
              rightSectionPointerEvents="none"
              onClick={() => combobox.toggleDropdown()}
            >
              {availableOperations.filter(
                (a) => a.value === form.values.operation
              )[0]?.name || <Input.Placeholder>Pick value</Input.Placeholder>}
            </InputBase>
          </Combobox.Target>

          <Combobox.Dropdown>
            <Combobox.Options>{availableOperationsOptions}</Combobox.Options>
          </Combobox.Dropdown>
        </Combobox>

        {form.values.operation ===
          availableOperations.filter((ao) => ao.name === "Custom")[0].value && (
          <TextInput
            key={form.key("custom")}
            label="Custom"
            required
            {...form.getInputProps("custom")}
          />
        )}

        <TextInput
          key={form.key("value2")}
          label="Value 2"
          required
          {...form.getInputProps("value2")}
        />

        <Button type="submit">Submit</Button>
      </Stack>
      {lastOperation && (
        <Stack>
          <Title>Last Result</Title>
          <OperationView
            value1={lastOperation.value1 ?? "undefined"}
            value2={lastOperation.value2 ?? "undefined"}
            operation={lastOperation.operation ?? "undefined"}
            custom={lastOperation.custom ?? "undefined"}
            result={lastOperation.result ?? "undefined"}
          />
        </Stack>
      )}
      <Stack>
        <Title>History</Title>
        {user.operations.map((o, index) => (
          <OperationView
            value1={o.value1 ?? "undefined"}
            value2={o.value2 ?? "undefined"}
            operation={o.operation ?? "undefined"}
            custom={o.custom ?? "undefined"}
            result={o.result ?? "undefined"}
            key={index}
          />
        ))}
      </Stack>
    </Stack>
  );
};

export default App;
