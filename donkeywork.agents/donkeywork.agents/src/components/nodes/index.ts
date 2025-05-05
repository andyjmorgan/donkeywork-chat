import BaseNode from './BaseNode';
import InputNode from './InputNode';
import ModelNode from './ModelNode';
import OutputNode from './OutputNode';
import StringFormatterNode from './StringFormatterNode';
import ConditionalNode from './ConditionalNode';
import TemplateNode from './TemplateNode';

export { BaseNode, InputNode, ModelNode, OutputNode, StringFormatterNode, ConditionalNode, TemplateNode };

export const nodeTypes = {
  inputNode: InputNode,
  modelNode: ModelNode,
  outputNode: OutputNode,
  stringFormatterNode: StringFormatterNode,
  conditionalNode: ConditionalNode,
  templateNode: TemplateNode,
};