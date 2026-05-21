<template>
  <div>
    <h2>Pacientes</h2>
    <table v-if="pacientes.length">
      <thead>
        <tr>
          <th>#</th>
          <th>Nombre</th>
          <th>DNI</th>
          <th>Email</th>
          <th>Teléfono</th>
          <th>No-shows</th>
          <th>Bloqueado</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="p in pacientes" :key="p.id">
          <td>{{ p.id }}</td>
          <td>{{ p.nombreCompleto }}</td>
          <td>{{ p.dni }}</td>
          <td>{{ p.email }}</td>
          <td>{{ p.telefono }}</td>
          <td>{{ p.noShowCount }}</td>
          <td>
            <span v-if="p.bloqueado" style="color: #d32f2f; font-weight: 600">Sí</span>
            <span v-else style="color: #388e3c">No</span>
          </td>
          <td>
            <button class="btn-danger" @click="eliminar(p.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>No hay pacientes registrados.</p>
  </div>
</template>

<script>
import { pacientesApi } from '../services/api'

export default {
  name: 'PacientesList',
  data() {
    return {
      pacientes: []
    }
  },
  async mounted() {
    try {
      const res = await pacientesApi.getAll()
      this.pacientes = res.data
    } catch {
      alert('Error al procesar la solicitud')
    }
  },
  methods: {
    async eliminar(id) {
      if (!confirm('¿Estás seguro que querés eliminar este paciente?')) return
      try {
        await pacientesApi.delete(id)
        this.pacientes = this.pacientes.filter(p => p.id !== id)
      } catch (e) {
        alert(e.response?.data?.mensaje || 'Error al eliminar el paciente')
      }
    }
  }
}
</script>
