import cluster from "node:cluster";

if (cluster.isPrimary) {
    cluster.fork();
    cluster.fork();
} else {
    await import("./server.js");
}