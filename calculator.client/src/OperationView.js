import { Group, TextInput } from "@mantine/core";
import React from "react";

export function OperationView({ value1, value2, operation, custom, result }) {
  return (
    <Group>
      <TextInput readOnly value={value1} label="Value 1" />
      <TextInput readOnly value={operation} label="Operation" />
      <TextInput readOnly value={value2} label="Value 2" />
      <TextInput readOnly value={custom} label="Operation" />
      <TextInput readOnly value={result} label="Result" />
    </Group>
  );
}
