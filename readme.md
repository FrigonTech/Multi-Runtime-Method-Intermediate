## 🔁 MRMI – Multi-Runtime Method Intermediate

**MRMI** (Multi-Runtime Method Intermediate) is an original cross-runtime communication technique developed by **Frigon Tech**. It enables one compiled program to invoke functions in another compiled module — regardless of language — using socket-based messaging and a standardized, typed command format.

### ⚙️ How It Works

- A calling runtime sends a structured command like:  
  `FunctionName-strHello,int42,booltrue`

- The receiving runtime (e.g., Java) parses this, maps data types (`str`, `int`, `bool`), and invokes the target method using reflection.

- The result (or error) is sent back as a serialized string prefixed with its type (e.g., `stringSuccess`, `int8080`).

- All requests and responses are logged with timestamps for traceability.

### ✅ Features

- Cross-language function calls via sockets
- Typed parameter encoding using lightweight syntax
- Reflection-based method resolution and execution
- Built-in logging and error handling
- Easily extendable to any language with socket + reflection support

### 💡 Use Case

MRMI is ideal for systems where modules written in different languages need to interoperate at runtime, without recompilation or tight integration — enabling flexible, plug-and-play architecture across platforms.

> Developed and maintained by **Frigon Tech**  
> GitHub: [github.com/FrigonTech](https://github.com/FrigonTech)
