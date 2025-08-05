## 🔁 MRMI – Multi-Runtime Method Intermediate
<p align="center">
  <img width="512" height="512" alt="MRMI" src="https://github.com/user-attachments/assets/beb1f4e6-7777-4ec6-af28-1b67a44ad649" />
</p>


**MRMI** (Multi-Runtime Method Intermediate) is an original cross-runtime communication technique developed by **Frigon Tech**. It enables one compiled program to invoke functions in another compiled module — regardless of language — using socket-based messaging and a standardized, typed command format.

### ⚙️ How It Works
- The independent script works as the listener and the controlled script (part of the main program) works as a client making requests and getting return values of independent script in return.
  
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

MRMI is ideal for *POLYGLOT* systems where modules written in different languages need to interoperate at runtime, without recompilation or tight integration — enabling flexible, plug-and-play architecture across platforms.
Has only been tested for non-obfuscated code yet in a windows environment, same thing with few tweaks may work on Mac and Linux too. Working with obfuscated code is still in progress

> Developed and maintained by **Frigon Tech**  
> GitHub: [github.com/FrigonTech](https://github.com/FrigonTech)
