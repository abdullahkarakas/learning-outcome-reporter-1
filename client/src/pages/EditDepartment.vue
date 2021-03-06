<template>
  <q-page class="flex row justify-center content-start items-center q-col-gutter-lg" padding>
    <div class="row justify-center col-md-12 col-lg-4">
      <div class="col-12">
        <q-card class="q-pa-sm">
          <q-card-section>
            <span class="text-h5">Edit Department</span>
          </q-card-section>

          <q-card-section>
            <q-input v-model="department.Name" label="Name" :loading="loading"></q-input>
          </q-card-section>

          <q-card-actions class="justify-end">
            <q-btn color="primary" @click="onUpdate" :loading="loading">Update</q-btn>
          </q-card-actions>
        </q-card>
      </div>
    </div>
    <o-crud-table
      class="q-my-lg col-md-12 col-lg-8"
      :entity="entity"
      :data="items"
      :columns="columns"
      :pagination="pagination"
      :actions="actionConfig"
    >
      <template v-slot:form="{ item }">
        <q-input v-model.number="item.Code" label="Code" type="number" min="1" max="255"></q-input>
        <q-input v-model="item.Description" label="Description" autogrow></q-input>
      </template>
    </o-crud-table>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, onMounted } from '@vue/composition-api'
import { Notify } from 'quasar'

import OCrudTable from '../components/OCrudTable'
import { ODataApiService } from '../services/ApiService'

export default defineComponent({
  name: 'EditDepartmentPage',
  components: {
    OCrudTable
  },
  setup (props, context) {
    const items = ref([])
    const columns = ref([
      {
        name: 'code',
        label: 'Code',
        field: 'Code',
        sortable: true,
        searchable: true
      },
      {
        name: 'description',
        label: 'Description',
        field: 'Description',
        sortable: true,
        searchable: true
      },
      { name: 'actions', label: 'Actions', align: 'right' }
    ])
    const pagination = reactive({
      page: 1,
      sortBy: 'code',
      descending: false,
      rowsPerPage: context.root.$q.screen.xs ? 12 : 24,
      rowsNumber: 0
    })

    const departmentId = context.root.$route.params.id

    const programOutcomeService = new ODataApiService(`/api/Departments/${departmentId}/Outcomes`)

    const entity = {
      key: 'Id',
      name: 'ProgramOutcome',
      displayName: (plural = false) => `Program Outcome${plural ? 's' : ''}`,
      route: (key = '') => `/departments/${departmentId}/outcomes/${key}`,
      service: programOutcomeService,
      defaultValue () {
        return {
          Id: 0,
          Code: 1,
          Description: ''
        }
      }
    }

    const actionConfig = {
      create: {
        icon: 'mdi-plus'
      },
      edit: {
        enabled: false
      }
    }

    const departmentService = new ODataApiService('/api/Departments')

    const { loading, onUpdate, department } = useUpdateForm(departmentService, departmentId)

    return {
      items,
      columns,
      pagination,
      entity,
      actionConfig,

      loading,
      onUpdate,
      department
    }
  }
})

function useUpdateForm (departmentService, departmentId) {
  const loading = ref(false)

  const department = reactive({
    Id: 0,
    Name: ''
  })

  onMounted(async () => {
    loading.value = true

    try {
      const data = await departmentService.get(departmentId)
      Object.assign(department, data)
    } catch (error) {
      Notify.create({
        type: 'negative',
        position: 'top',
        message: 'An error occured while fetching the data',
        caption: error.message
      })

      return
    } finally {
      loading.value = false
    }
  })

  const onUpdate = async () => {
    loading.value = true

    try {
      await departmentService.update(departmentId, department)
    } catch (error) {
      Notify.create({
        type: 'negative',
        position: 'top',
        message: 'An error occured while updating',
        caption: error.message
      })

      return
    } finally {
      loading.value = false
    }

    Notify.create({
      type: 'positive',
      position: 'top',
      message: 'Update successful'
    })
  }

  return {
    loading,
    onUpdate,
    department
  }
}
</script>
