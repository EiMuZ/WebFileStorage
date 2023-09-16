<script setup>

import axios from "axios";
import {ref} from "vue";

let username = ref()
let password = ref()

function signIn() {
  axios({
    url: '/api/identity/signin',
    method: 'post',
    headers: {
      'Content-Type': 'application/json'
    },
    data: JSON.stringify({
      username: username.value,
      password: password.value
    })
  }).then(res => {
    if (res.status !== 200 || res.data.code !== 0) {
      throw ''
    }
    localStorage.setItem('token', res.data.data.token)
  }).catch(() => {
    console.log('sign-in failed')
  })
}

</script>

<template>
<div class="screen">
  <div class="panel">
    <input type="text" placeholder="username" v-model="username">
    <input type="password" placeholder="password" v-model="password">
    <button type="button" @click="signIn">sign in</button>
  </div>
</div>
</template>

<style scoped>
.screen {
  width: 100vw;
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
}

.panel {
  display: flex;
  flex-direction: column;
  padding: 2rem;
  background-color: rgba(0, 255, 255, 0.3);
  backdrop-filter: blur(3px);
  border-radius: 0.5rem;
}

.panel > * {
  font-size: 1.1rem;
  padding: 0.3rem 0.6rem;
  margin: 0.3rem;
}
</style>