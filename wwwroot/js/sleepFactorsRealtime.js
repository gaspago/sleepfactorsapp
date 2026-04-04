window.sleepFactorsRealtime = (() => {
  const map = new Map();

  async function start(key, dotNetRef) {
    if (map.has(key)) {
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/sleep-factors")
      .withAutomaticReconnect()
      .build();

    connection.on("DailyLogSaved", async () => {
      await dotNetRef.invokeMethodAsync("HandleRealtimeRefresh");
    });

    await connection.start();
    map.set(key, connection);
  }

  async function stop(key) {
    const connection = map.get(key);
    if (!connection) {
      return;
    }

    await connection.stop();
    map.delete(key);
  }

  return {
    start,
    stop
  };
})();
