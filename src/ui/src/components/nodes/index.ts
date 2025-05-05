import BaseNode from './BaseNode';
import InputNode from './InputNode';
import ModelNode from './ModelNode';
import OutputNode from './OutputNode';
import StringFormatterNode from './StringFormatterNode';
import ConditionalNode from './ConditionalNode';

export { BaseNode, InputNode, ModelNode, OutputNode, StringFormatterNode, ConditionalNode };

export const nodeTypes = {
  inputNode: InputNode,
  modelNode: ModelNode,
  outputNode: OutputNode,
  stringFormatterNode: StringFormatterNode,
  conditionalNode: ConditionalNode,
};